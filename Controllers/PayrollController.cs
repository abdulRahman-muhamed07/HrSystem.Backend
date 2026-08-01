using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Data;
using HrSystem.Backend.Models;
using HrSystem.Backend.Models.Dtos;
using HrSystem.Backend.Services;
using System.Security.Claims;

namespace HrSystem.Backend.Controllers;

[Route("api/Areas/Admin/Payroll")]
[ApiController]
[Authorize]
public class PayrollController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPayrollEngine _payrollEngine;
    private readonly IAuditService _audit;

    public PayrollController(AppDbContext db, IPayrollEngine payrollEngine, IAuditService audit)
    {
        _db = db;
        _payrollEngine = payrollEngine;
        _audit = audit;
    }

    /// <summary>
    /// GET api/Areas/Admin/Payroll?year=2026&month=8&status=Paid
    /// List payroll records.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] int? year, [FromQuery] int? month, [FromQuery] string? status)
    {
        var query = _db.PayrollRecords
            .Include(p => p.Employee)
            .AsQueryable();

        if (year.HasValue) query = query.Where(p => p.Year == year);
        if (month.HasValue) query = query.Where(p => p.Month == month);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(p => p.Status == status);

        var items = await query
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .ThenBy(p => p.Employee!.FullName)
            .ToListAsync();

        return Ok(items.Select(ToDto));
    }

    /// <summary>
    /// GET api/Areas/Admin/Payroll/{id}
    /// Payslip for one record.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PayrollDto>> GetById(int id)
    {
        var p = await _db.PayrollRecords
            .Include(p => p.Employee)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (p == null)
            return NotFound(new { message = "Payroll record not found" });
        return Ok(ToDto(p));
    }

    /// <summary>
    /// GET api/Areas/Admin/Payroll/mine?year=2026&month=8
    /// Current employee's payslip for a month (defaults to previous month).
    /// </summary>
    [HttpGet("mine")]
    public async Task<ActionResult> GetMine([FromQuery] int? year, [FromQuery] int? month)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await _db.Users.FindAsync(userId);
        if (user?.EmployeeId == null)
            return BadRequest(new { message = "No employee profile linked to this account." });

        var y = year ?? DateTime.Today.Year;
        var m = month ?? DateTime.Today.Month;

        var p = await _db.PayrollRecords
            .Include(p => p.Employee)
            .FirstOrDefaultAsync(p => p.EmployeeId == user.EmployeeId && p.Year == y && p.Month == m);

        if (p == null)
            return NotFound(new { message = "No payslip found for the selected month." });

        return Ok(ToDto(p));
    }

    /// <summary>
    /// POST api/Areas/Admin/Payroll/run
    /// Run the monthly payroll. Computes GOSI, income tax, overtime and loan deductions
    /// for all active employees (or a single employee when employeeId is provided).
    /// </summary>
    [Authorize(Roles = "Admin,HR")]
    [HttpPost("run")]
    public async Task<ActionResult<PayrollRunResultDto>> Run([FromBody] PayrollRunRequestDto dto)
    {
        if (dto.Year < 2000 || dto.Year > 2100 || dto.Month < 1 || dto.Month > 12)
            return BadRequest(new { message = "Invalid year or month." });

        var employeesQuery = _db.Employees.AsQueryable();
        if (dto.EmployeeId.HasValue)
            employeesQuery = employeesQuery.Where(e => e.Id == dto.EmployeeId);
        else
            employeesQuery = employeesQuery.Where(e => e.EmploymentStatus == "Active");

        var employees = await employeesQuery.ToListAsync();
        if (employees.Count == 0)
            return NotFound(new { message = "No active employees found to process." });

        var monthStart = new DateTime(dto.Year, dto.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        // Approved overtime within the month (aggregate in memory — SQLite can't SUM decimals)
        var approvedOvertime = await _db.OvertimeRequests
            .Where(o => o.Status == "Approved" && o.Date >= monthStart && o.Date <= monthEnd)
            .ToListAsync();
        var overtimeMap = approvedOvertime
            .GroupBy(o => o.EmployeeId)
            .ToDictionary(g => g.Key, g => g.Sum(o => o.Hours * o.RateMultiplier));

        // Approved, unpaid loans
        var loans = await _db.EmployeeLoans
            .Where(l => l.Status == "Approved" && l.RemainingAmount > 0)
            .ToListAsync();

        var result = new PayrollRunResultDto();
        var records = new List<PayrollRecord>();
        var otherAllowances = dto.OtherAllowances ?? 0;
        var otherDeductions = dto.OtherDeductions ?? 0;

        foreach (var emp in employees)
        {
            // Skip if a record already exists for this employee/month
            var existing = await _db.PayrollRecords
                .FirstOrDefaultAsync(p => p.EmployeeId == emp.Id && p.Year == dto.Year && p.Month == dto.Month);
            if (existing != null)
            {
                records.Add(existing);
                continue;
            }

            // Overtime pay = hourly basic rate x hours x multiplier
            overtimeMap.TryGetValue(emp.Id, out var otMultipliers);
            var hourlyRate = emp.Salary / 30m / 8m;
            var overtimePay = Math.Round(hourlyRate * otMultipliers, 2);

            // Loan deduction for this month (sum of monthly deductions, capped by remaining)
            var empLoans = loans.Where(l => l.EmployeeId == emp.Id).ToList();
            var loanDeduction = empLoans.Sum(l => Math.Min(l.MonthlyDeduction, l.RemainingAmount));

            var calc = _payrollEngine.Compute(emp, overtimePay, otherAllowances, otherDeductions, loanDeduction);

            var record = new PayrollRecord
            {
                EmployeeId = emp.Id,
                Year = dto.Year,
                Month = dto.Month,
                BasicSalary = calc.BasicSalary,
                HousingAllowance = calc.HousingAllowance,
                TransportationAllowance = calc.TransportationAllowance,
                MealAllowance = calc.MealAllowance,
                OtherAllowances = calc.OtherAllowances,
                OvertimePay = calc.OvertimePay,
                GrossSalary = calc.GrossSalary,
                GosiEmployee = calc.GosiEmployee,
                GosiEmployer = calc.GosiEmployer,
                IncomeTax = calc.IncomeTax,
                LoanDeduction = calc.LoanDeduction,
                OtherDeductions = calc.OtherDeductions,
                NetSalary = calc.NetSalary,
                Status = "Draft"
            };

            _db.PayrollRecords.Add(record);
            records.Add(record);

            // Apply loan deductions to remaining balances
            foreach (var loan in empLoans)
            {
                loan.RemainingAmount -= Math.Min(loan.MonthlyDeduction, loan.RemainingAmount);
                if (loan.RemainingAmount <= 0)
                {
                    loan.RemainingAmount = 0;
                    loan.Status = "Completed";
                }
            }
        }

        await _db.SaveChangesAsync();

        result.Processed = records.Count;
        result.TotalGross = records.Sum(r => r.GrossSalary);
        result.TotalNet = records.Sum(r => r.NetSalary);
        result.Records = records.Select(ToDto).ToList();

        await _audit.LogAsync("RunPayroll", "Payroll", $"{dto.Year}-{dto.Month}",
            $"Ran payroll for {records.Count} employees. Total gross {result.TotalGross}, net {result.TotalNet}");

        return Ok(result);
    }

    /// <summary>
    /// PUT api/Areas/Admin/Payroll/{id}/pay
    /// Mark a payroll record as Paid.
    /// </summary>
    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}/pay")]
    public async Task<IActionResult> MarkPaid(int id)
    {
        var record = await _db.PayrollRecords.FindAsync(id);
        if (record == null)
            return NotFound(new { message = "Payroll record not found" });

        if (record.Status == "Paid")
            return BadRequest(new { message = "Record is already marked as paid." });

        record.Status = "Paid";
        record.PaidAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        await _audit.LogAsync("MarkPaid", "Payroll", record.Id.ToString(),
            $"Marked payroll {record.Year}-{record.Month} for employee {record.EmployeeId} as paid");

        return Ok(new { message = "Payroll marked as paid." });
    }

    /// <summary>
    /// GET api/Areas/Admin/Payroll/summary?year=2026&month=8
    /// Monthly payroll totals.
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult> Summary([FromQuery] int year, [FromQuery] int month)
    {
        var records = await _db.PayrollRecords
            .Where(p => p.Year == year && p.Month == month)
            .ToListAsync();

        return Ok(new
        {
            Year = year,
            Month = month,
            EmployeeCount = records.Count,
            TotalBasic = records.Sum(r => r.BasicSalary),
            TotalAllowances = records.Sum(r => r.HousingAllowance + r.TransportationAllowance + r.MealAllowance + r.OtherAllowances),
            TotalOvertime = records.Sum(r => r.OvertimePay),
            TotalGross = records.Sum(r => r.GrossSalary),
            TotalGosiEmployee = records.Sum(r => r.GosiEmployee),
            TotalGosiEmployer = records.Sum(r => r.GosiEmployer),
            TotalIncomeTax = records.Sum(r => r.IncomeTax),
            TotalLoanDeductions = records.Sum(r => r.LoanDeduction),
            TotalOtherDeductions = records.Sum(r => r.OtherDeductions),
            TotalNet = records.Sum(r => r.NetSalary)
        });
    }

    private static PayrollDto ToDto(PayrollRecord p) => new()
    {
        Id = p.Id,
        EmployeeId = p.EmployeeId,
        EmployeeName = p.Employee?.FullName ?? "Unknown",
        Year = p.Year,
        Month = p.Month,
        BasicSalary = p.BasicSalary,
        HousingAllowance = p.HousingAllowance,
        TransportationAllowance = p.TransportationAllowance,
        MealAllowance = p.MealAllowance,
        OtherAllowances = p.OtherAllowances,
        OvertimePay = p.OvertimePay,
        GrossSalary = p.GrossSalary,
        GosiEmployee = p.GosiEmployee,
        GosiEmployer = p.GosiEmployer,
        IncomeTax = p.IncomeTax,
        LoanDeduction = p.LoanDeduction,
        OtherDeductions = p.OtherDeductions,
        NetSalary = p.NetSalary,
        Status = p.Status,
        PaidAt = p.PaidAt?.ToString("yyyy-MM-dd HH:mm")
    };
}
