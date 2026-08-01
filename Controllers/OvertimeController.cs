using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Data;
using HrSystem.Backend.Models;
using HrSystem.Backend.Models.Dtos;
using HrSystem.Backend.Services;
using System.Security.Claims;

namespace HrSystem.Backend.Controllers;

[Route("api/Areas/Admin/Overtime")]
[ApiController]
[Authorize]
public class OvertimeController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;

    public OvertimeController(AppDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    /// <summary>
    /// GET api/Areas/Admin/Overtime
    /// All overtime requests (Admin/HR). Optional ?status= filter.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] string? status)
    {
        var query = _db.OvertimeRequests
            .Include(o => o.Employee)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(o => o.Status == status);

        var items = await query
            .OrderByDescending(o => o.Date)
            .ThenByDescending(o => o.CreatedAt)
            .ToListAsync();

        return Ok(items.Select(o => ToDto(o)));
    }

    /// <summary>
    /// GET api/Areas/Admin/Overtime/my
    /// Current user's overtime requests.
    /// </summary>
    [HttpGet("my")]
    public async Task<ActionResult> GetMy()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await _db.Users.FindAsync(userId);
        if (user?.EmployeeId == null)
            return BadRequest(new { message = "No employee profile linked to this account." });

        var items = await _db.OvertimeRequests
            .Where(o => o.EmployeeId == user.EmployeeId)
            .Include(o => o.Employee)
            .OrderByDescending(o => o.Date)
            .ToListAsync();

        return Ok(items.Select(o => ToDto(o)));
    }

    /// <summary>
    /// POST api/Areas/Admin/Overtime
    /// Submit an overtime request.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OvertimeDto>> Create([FromBody] OvertimeCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return Unauthorized(new { message = "User not found" });
        if (user.EmployeeId == null)
            return BadRequest(new { message = "No employee profile linked to this account." });

        if (!DateTime.TryParse(dto.Date, out var date))
            return BadRequest(new { message = "Invalid date." });
        if (dto.Hours <= 0 || dto.Hours > 24)
            return BadRequest(new { message = "Hours must be between 0 and 24." });
        if (dto.RateMultiplier <= 0)
            return BadRequest(new { message = "Invalid rate multiplier." });

        var request = new OvertimeRequest
        {
            EmployeeId = user.EmployeeId.Value,
            Date = date,
            Hours = dto.Hours,
            RateMultiplier = dto.RateMultiplier,
            Reason = dto.Reason,
            Status = "Pending"
        };

        _db.OvertimeRequests.Add(request);
        await _db.SaveChangesAsync();

        await _db.Entry(request).Reference(o => o.Employee).LoadAsync();

        await _audit.LogAsync("Create", "OvertimeRequest", request.Id.ToString(),
            $"Overtime {dto.Hours}h x{dto.RateMultiplier} on {date:yyyy-MM-dd}");

        return Ok(ToDto(request));
    }

    /// <summary>
    /// PUT api/Areas/Admin/Overtime/{id}/status
    /// Approve/reject an overtime request (Admin/HR).
    /// </summary>
    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] OvertimeStatusDto dto)
    {
        if (dto.Status != "Approved" && dto.Status != "Rejected")
            return BadRequest(new { message = "Status must be 'Approved' or 'Rejected'." });

        var request = await _db.OvertimeRequests
            .Include(o => o.Employee)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (request == null)
            return NotFound(new { message = "Overtime request not found" });

        if (request.Status != "Pending")
            return BadRequest(new { message = "This request has already been processed." });

        var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        request.Status = dto.Status;
        request.ApprovedBy = adminId;
        request.ApprovedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        await _audit.LogAsync(dto.Status == "Approved" ? "Approve" : "Reject", "OvertimeRequest", request.Id.ToString(),
            $"{dto.Status} overtime {request.Hours}h for {request.Employee?.FullName}");

        return Ok(new { message = $"Overtime request {dto.Status.ToLower()}." });
    }

    /// <summary>
    /// DELETE api/Areas/Admin/Overtime/{id}
    /// Cancel a pending overtime request.
    /// </summary>
    [Authorize(Roles = "Admin,HR")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var request = await _db.OvertimeRequests.FindAsync(id);
        if (request == null)
            return NotFound(new { message = "Overtime request not found" });

        if (request.Status == "Approved")
            return BadRequest(new { message = "Cannot delete an approved overtime request." });

        _db.OvertimeRequests.Remove(request);
        await _db.SaveChangesAsync();

        await _audit.LogAsync("Delete", "OvertimeRequest", id.ToString(), $"Deleted overtime request");

        return NoContent();
    }

    private OvertimeDto ToDto(OvertimeRequest o)
    {
        var hourlyRate = o.Employee != null ? o.Employee.Salary / 30m / 8m : 0;
        return new OvertimeDto
        {
            Id = o.Id,
            EmployeeId = o.EmployeeId,
            EmployeeName = o.Employee?.FullName ?? "Unknown",
            Date = o.Date.ToString("yyyy-MM-dd"),
            Hours = o.Hours,
            RateMultiplier = o.RateMultiplier,
            EstimatedPay = Math.Round(hourlyRate * o.Hours * o.RateMultiplier, 2),
            Reason = o.Reason,
            Status = o.Status,
            CreatedAt = o.CreatedAt.ToString("yyyy-MM-dd HH:mm")
        };
    }
}
