using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Data;
using HrSystem.Backend.Models;
using HrSystem.Backend.Models.Dtos;
using HrSystem.Backend.Services;
using System.Security.Claims;

namespace HrSystem.Backend.Controllers;

[Route("api/Areas/Admin/LeaveBalances")]
[ApiController]
[Authorize]
public class LeaveBalancesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;
    private readonly ILeaveBalanceService _balanceService;

    public LeaveBalancesController(AppDbContext db, IAuditService audit, ILeaveBalanceService balanceService)
    {
        _db = db;
        _audit = audit;
        _balanceService = balanceService;
    }

    /// <summary>
    /// GET api/Areas/Admin/LeaveBalances?year=2026
    /// All employee leave balances for a year (defaults to current year).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] int? year)
    {
        var y = year ?? DateTime.Today.Year;

        var balances = await _db.EmployeeLeaveBalances
            .Include(b => b.Employee)
            .Include(b => b.LeaveType)
            .Where(b => b.Year == y)
            .OrderBy(b => b.Employee!.FullName)
            .ToListAsync();

        return Ok(balances.Select(ToDto));
    }

    /// <summary>
    /// GET api/Areas/Admin/LeaveBalances/my
    /// Current user's leave balances.
    /// </summary>
    [HttpGet("my")]
    public async Task<ActionResult> GetMy()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await _db.Users.FindAsync(userId);
        if (user?.EmployeeId == null)
            return BadRequest(new { message = "No employee profile linked to this account." });

        var y = DateTime.Today.Year;
        var balances = await _db.EmployeeLeaveBalances
            .Include(b => b.Employee)
            .Include(b => b.LeaveType)
            .Where(b => b.EmployeeId == user.EmployeeId && b.Year == y)
            .ToListAsync();

        return Ok(balances.Select(ToDto));
    }

    /// <summary>
    /// GET api/Areas/Admin/LeaveBalances/employee/{employeeId}
    /// Balances for one employee.
    /// </summary>
    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult> GetForEmployee(int employeeId, [FromQuery] int? year)
    {
        var y = year ?? DateTime.Today.Year;
        var balances = await _db.EmployeeLeaveBalances
            .Include(b => b.Employee)
            .Include(b => b.LeaveType)
            .Where(b => b.EmployeeId == employeeId && b.Year == y)
            .ToListAsync();

        return Ok(balances.Select(ToDto));
    }

    /// <summary>
    /// PUT api/Areas/Admin/LeaveBalances/{id}/adjust
    /// Admin adjusts an employee's balance (e.g. bonus days / corrections).
    /// </summary>
    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}/adjust")]
    public async Task<IActionResult> Adjust(int id, [FromBody] LeaveBalanceAdjustDto dto)
    {
        var balance = await _db.EmployeeLeaveBalances
            .Include(b => b.Employee)
            .Include(b => b.LeaveType)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (balance == null)
            return NotFound(new { message = "Balance not found" });

        balance.AdjustedDays += dto.AdjustedDays;
        await _db.SaveChangesAsync();

        await _audit.LogAsync("AdjustBalance", "LeaveBalance", balance.Id.ToString(),
            $"Adjusted {balance.Employee?.FullName} {balance.LeaveType?.Name} balance by {dto.AdjustedDays} days. {dto.Note}");

        return Ok(new
        {
            message = "Balance adjusted.",
            RemainingDays = balance.EntitledDays - balance.UsedDays + balance.AdjustedDays
        });
    }

    /// <summary>
    /// POST api/Areas/Admin/LeaveBalances/refresh
    /// Ensures balances exist for the given year for all employees/leave types.
    /// </summary>
    [Authorize(Roles = "Admin,HR")]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromQuery] int? year)
    {
        var y = year ?? DateTime.Today.Year;
        var employees = await _db.Employees.Where(e => e.EmploymentStatus == "Active").ToListAsync();
        var leaveTypes = await _db.LeaveTypes.Where(t => t.IsActive).ToListAsync();

        int created = 0;
        foreach (var emp in employees)
        {
            foreach (var lt in leaveTypes)
            {
                var existing = await _db.EmployeeLeaveBalances
                    .AnyAsync(b => b.EmployeeId == emp.Id && b.LeaveTypeId == lt.Id && b.Year == y);
                if (!existing)
                {
                    await _balanceService.EnsureBalanceAsync(_db, emp.Id, lt.Id, y);
                    created++;
                }
            }
        }
        await _db.SaveChangesAsync();

        await _audit.LogAsync("RefreshBalances", "LeaveBalance", null, $"Created {created} balance rows for year {y}");

        return Ok(new { message = "Balances refreshed.", Created = created });
    }

    private static LeaveBalanceDto ToDto(EmployeeLeaveBalance b) => new()
    {
        Id = b.Id,
        EmployeeId = b.EmployeeId,
        EmployeeName = b.Employee?.FullName ?? "Unknown",
        LeaveTypeId = b.LeaveTypeId,
        LeaveTypeName = b.LeaveType?.Name,
        LeaveTypeNameAr = b.LeaveType?.NameAr,
        Year = b.Year,
        EntitledDays = b.EntitledDays,
        UsedDays = b.UsedDays,
        AdjustedDays = b.AdjustedDays,
        RemainingDays = b.EntitledDays - b.UsedDays + b.AdjustedDays
    };
}
