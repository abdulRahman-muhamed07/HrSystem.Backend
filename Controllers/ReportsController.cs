using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Data;
using HrSystem.Backend.Services;

namespace HrSystem.Backend.Controllers;

[Route("api/Areas/Admin/Reports")]
[ApiController]
[Authorize(Roles = "Admin,HR")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ReportsController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// GET api/Areas/Admin/Reports/attendance?from=2026-08-01&to=2026-08-31&employeeId=&departmentId=&format=json|csv
    /// Attendance report.
    /// </summary>
    [HttpGet("attendance")]
    public async Task<IActionResult> Attendance([FromQuery] string? from, [FromQuery] string? to,
        [FromQuery] int? employeeId, [FromQuery] int? departmentId, [FromQuery] string format = "json")
    {
        var fromDate = ParseDate(from) ?? DateTime.Today.AddMonths(-1);
        var toDate = ParseDate(to) ?? DateTime.Today;

        var query = _db.AttendanceRecords
            .Include(a => a.Employee)
                .ThenInclude(e => e!.Department)
            .Where(a => a.Date >= fromDate && a.Date <= toDate)
            .AsQueryable();

        if (employeeId.HasValue) query = query.Where(a => a.EmployeeId == employeeId);
        if (departmentId.HasValue) query = query.Where(a => a.Employee!.DepartmentId == departmentId);

        var rows = await query
            .OrderBy(a => a.Date).ThenBy(a => a.Employee!.FullName)
            .ToListAsync();

        var headers = new[] { "Employee", "Department", "Date", "Check In", "Check Out", "Hours", "Status" };
        var data = rows.Select(a =>
        {
            var hours = a.CheckIn.HasValue && a.CheckOut.HasValue
                ? Math.Round((a.CheckOut.Value - a.CheckIn.Value).TotalHours, 2).ToString()
                : "";
            return new[] { a.Employee?.FullName ?? "Unknown", a.Employee?.Department?.Name ?? "-",
                a.Date.ToString("yyyy-MM-dd"), a.CheckIn?.ToString("hh:mm tt") ?? "-",
                a.CheckOut?.ToString("hh:mm tt") ?? "-", hours, a.Status };
        });

        return FormatResult(format, headers, data,
            $"attendance_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.csv");
    }

    /// <summary>
    /// GET api/Areas/Admin/Reports/leaves?from=2026-08-01&to=2026-08-31&employeeId=&status=&format=json|csv
    /// Leave report.
    /// </summary>
    [HttpGet("leaves")]
    public async Task<IActionResult> Leaves([FromQuery] string? from, [FromQuery] string? to,
        [FromQuery] int? employeeId, [FromQuery] string? status, [FromQuery] string format = "json")
    {
        var fromDate = ParseDate(from) ?? DateTime.Today.AddMonths(-3);
        var toDate = ParseDate(to) ?? DateTime.Today;

        var query = _db.LeaveRequests
            .Include(l => l.Employee).ThenInclude(e => e!.Department)
            .Include(l => l.LeaveType)
            .Where(l => l.StartDate >= fromDate && l.EndDate <= toDate)
            .AsQueryable();

        if (employeeId.HasValue) query = query.Where(l => l.EmployeeId == employeeId);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(l => l.Status == status);

        var rows = await query
            .OrderBy(l => l.StartDate).ThenBy(l => l.Employee!.FullName)
            .ToListAsync();

        var headers = new[] { "Employee", "Department", "Leave Type", "Start", "End", "Days", "Status", "Reason" };
        var data = rows.Select(l => new[] {
            l.Employee?.FullName ?? "Unknown", l.Employee?.Department?.Name ?? "-",
            l.LeaveType?.Name ?? "-", l.StartDate.ToString("yyyy-MM-dd"), l.EndDate.ToString("yyyy-MM-dd"),
            l.DurationDays.ToString(), l.Status, l.Reason });

        return FormatResult(format, headers, data,
            $"leaves_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.csv");
    }

    /// <summary>
    /// GET api/Areas/Admin/Reports/payroll?year=2026&month=8&employeeId=&format=json|csv
    /// Payroll report for a month.
    /// </summary>
    [HttpGet("payroll")]
    public async Task<IActionResult> Payroll([FromQuery] int year, [FromQuery] int month,
        [FromQuery] int? employeeId, [FromQuery] string format = "json")
    {
        var query = _db.PayrollRecords
            .Include(p => p.Employee).ThenInclude(e => e!.Department)
            .Where(p => p.Year == year && p.Month == month)
            .AsQueryable();

        if (employeeId.HasValue) query = query.Where(p => p.EmployeeId == employeeId);

        var rows = await query.OrderBy(p => p.Employee!.FullName).ToListAsync();

        var headers = new[] { "Employee", "Department", "Basic", "Allowances", "Overtime", "Gross",
            "GOSI (Emp)", "GOSI (Employer)", "Tax", "Loans", "Other Ded.", "Net", "Status" };
        var data = rows.Select(p => new[] {
            p.Employee?.FullName ?? "Unknown", p.Employee?.Department?.Name ?? "-",
            p.BasicSalary.ToString("N2"), (p.HousingAllowance + p.TransportationAllowance + p.MealAllowance + p.OtherAllowances).ToString("N2"),
            p.OvertimePay.ToString("N2"), p.GrossSalary.ToString("N2"),
            p.GosiEmployee.ToString("N2"), p.GosiEmployer.ToString("N2"),
            p.IncomeTax.ToString("N2"), p.LoanDeduction.ToString("N2"), p.OtherDeductions.ToString("N2"),
            p.NetSalary.ToString("N2"), p.Status });

        return FormatResult(format, headers, data,
            $"payroll_{year}_{month:00}.csv");
    }

    /// <summary>
    /// GET api/Areas/Admin/Reports/employees?format=json|csv
    /// Employee directory / master data export.
    /// </summary>
    [HttpGet("employees")]
    public async Task<IActionResult> Employees([FromQuery] string format = "json")
    {
        var rows = await _db.Employees
            .Include(e => e.Department)
            .OrderBy(e => e.FullName)
            .ToListAsync();

        var headers = new[] { "Name", "Email", "National ID", "Phone", "Gender", "Department", "Job Title",
            "Employment Type", "Employment Status", "Basic Salary", "Housing", "Transport", "Meal",
            "Hire Date", "Bank", "Account" };
        var data = rows.Select(e => new[] {
            e.FullName, e.Email, e.NationalId ?? "", e.Phone ?? "", e.Gender ?? "",
            e.Department?.Name ?? "-", e.JobTitle, e.EmploymentType, e.EmploymentStatus,
            e.Salary.ToString("N2"), e.HousingAllowance.ToString("N2"), e.TransportationAllowance.ToString("N2"),
            e.MealAllowance.ToString("N2"), e.HireDate.ToString("yyyy-MM-dd"), e.BankName ?? "", e.BankAccountNumber ?? "" });

        return FormatResult(format, headers, data, "employees.csv");
    }

    private static DateTime? ParseDate(string? s) => DateTime.TryParse(s, out var d) ? d : (DateTime?)null;

    private IActionResult FormatResult(string format, string[] headers, IEnumerable<IEnumerable<string>> data, string fileName)
    {
        if (string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase))
        {
            var bytes = CsvExporter.Build(headers, data);
            return File(bytes, "text/csv; charset=utf-8", fileName);
        }

        var list = data.ToList();
        var jsonData = list.Select(row =>
        {
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < headers.Length; i++)
                dict[headers[i]] = row.ElementAt(i);
            return dict;
        });
        return Ok(jsonData);
    }
}
