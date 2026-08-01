using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Data;
using HrSystem.Backend.Models;
using HrSystem.Backend.Models.Dtos;
using HrSystem.Backend.Services;
using System.Security.Claims;

namespace HrSystem.Backend.Controllers;

[Route("api/Areas/Admin/Attendance")]
[ApiController]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;

    public AttendanceController(AppDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    /// <summary>
    /// GET api/Areas/Admin/Attendance
    /// Returns attendance records with employee names.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var records = await _db.AttendanceRecords
            .Include(a => a.Employee)
            .OrderByDescending(a => a.Date)
            .ThenByDescending(a => a.CheckIn)
            .Take(100) // Limit to last 100 records for performance
            .ToListAsync();

        var result = records.Select(a => new AttendanceDto
        {
            Id = a.Id,
            EmployeeName = a.Employee != null ? a.Employee.FullName : "Unknown",
            Date = a.Date.ToString("yyyy-MM-dd"),
            CheckIn = a.CheckIn?.ToString("hh:mm tt"),
            CheckOut = a.CheckOut?.ToString("hh:mm tt"),
            Status = a.Status
        }).ToList();

        return Ok(result);
    }

    /// <summary>
    /// POST api/Areas/Admin/Attendance/check-in
    /// Record check-in for the current user.
    /// </summary>
    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return Unauthorized(new { message = "User not found" });
        if (user.EmployeeId == null)
            return BadRequest(new { message = "No employee profile linked to this account." });

        var today = DateTime.Today;

        // Check if already checked in today
        var existing = await _db.AttendanceRecords
            .FirstOrDefaultAsync(a => a.EmployeeId == user.EmployeeId && a.Date == today);

        if (existing != null && existing.CheckIn.HasValue)
            return BadRequest(new { message = "Already checked in today." });

        var now = TimeOnly.FromDateTime(DateTime.Now);
        var status = now > new TimeOnly(9, 0) ? "متأخر" : "منتظم";

        if (existing != null)
        {
            existing.CheckIn = now;
            existing.Status = status;
        }
        else
        {
            var record = new AttendanceRecord
            {
                EmployeeId = user.EmployeeId.Value,
                Date = today,
                CheckIn = now,
                Status = status
            };
            _db.AttendanceRecords.Add(record);
        }

        await _db.SaveChangesAsync();
        await _audit.LogAsync("CheckIn", "Attendance", user.EmployeeId?.ToString(), $"Checked in at {now:hh:mm tt}");

        return Ok(new { message = "Check-in recorded successfully", time = now.ToString("hh:mm tt"), status });
    }

    /// <summary>
    /// POST api/Areas/Admin/Attendance/check-out
    /// Record check-out for the current user.
    /// </summary>
    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return Unauthorized(new { message = "User not found" });
        if (user.EmployeeId == null)
            return BadRequest(new { message = "No employee profile linked to this account." });

        var today = DateTime.Today;

        var record = await _db.AttendanceRecords
            .FirstOrDefaultAsync(a => a.EmployeeId == user.EmployeeId && a.Date == today);

        if (record == null || !record.CheckIn.HasValue)
            return BadRequest(new { message = "No check-in record found for today." });

        if (record.CheckOut.HasValue)
            return BadRequest(new { message = "Already checked out today." });

        record.CheckOut = TimeOnly.FromDateTime(DateTime.Now);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CheckOut", "Attendance", user.EmployeeId?.ToString(), $"Checked out at {record.CheckOut:hh:mm tt}");

        return Ok(new { message = "Check-out recorded successfully", time = record.CheckOut.Value.ToString("hh:mm tt") });
    }
}