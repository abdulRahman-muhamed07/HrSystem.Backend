using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Data;
using HrSystem.Backend.Models.Dtos;

namespace HrSystem.Backend.Controllers;

[Route("api/Areas/Admin/Dashboard")]
[ApiController]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// GET api/Areas/Admin/Dashboard
    /// Returns dashboard statistics.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<DashboardStatsDto>> GetStats()
    {
        var totalEmployees = await _db.Employees.CountAsync();
        var activeEmployees = await _db.Employees.CountAsync(e => e.EmploymentStatus == "Active");
        var totalDepartments = await _db.Departments.CountAsync();
        var pendingLeaves = await _db.LeaveRequests.CountAsync(l => l.Status == "Pending");
        var pendingOvertime = await _db.OvertimeRequests.CountAsync(o => o.Status == "Pending");
        var pendingLoans = await _db.EmployeeLoans.CountAsync(l => l.Status == "Pending");

        var today = DateTime.Today;
        var checkedInToday = await _db.AttendanceRecords
            .CountAsync(a => a.Date == today && a.CheckIn.HasValue);

        var currentMonth = today.Month;
        var currentYear = today.Year;
        var monthlyRecords = await _db.PayrollRecords
            .Where(p => p.Year == currentYear && p.Month == currentMonth)
            .ToListAsync();
        var monthlyPayroll = monthlyRecords.Sum(p => p.NetSalary);

        var attendanceRate = totalEmployees > 0
            ? Math.Round((double)checkedInToday / totalEmployees * 100, 1)
            : 0;

        return Ok(new DashboardStatsDto
        {
            TotalEmployees = totalEmployees,
            TotalDepartments = totalDepartments,
            PendingLeaves = pendingLeaves,
            AttendanceRate = attendanceRate,
            PresentToday = checkedInToday,
            TotalCheckedIn = checkedInToday,
            ActiveEmployees = activeEmployees,
            PendingOvertime = pendingOvertime,
            PendingLoans = pendingLoans,
            TotalMonthlyPayroll = monthlyPayroll
        });
    }
}
