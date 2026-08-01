using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Data;
using HrSystem.Backend.Models;
using HrSystem.Backend.Models.Dtos;
using HrSystem.Backend.Services;
using System.Security.Claims;

namespace HrSystem.Backend.Controllers;

[Route("api/Areas/Admin/Leaves")]
[ApiController]
[Authorize]
public class LeavesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;
    private readonly ILeaveBalanceService _balanceService;

    public LeavesController(AppDbContext db, IAuditService audit, ILeaveBalanceService balanceService)
    {
        _db = db;
        _audit = audit;
        _balanceService = balanceService;
    }

    /// <summary>
    /// GET api/Areas/Admin/Leaves
    /// Returns all leave requests with employee names.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var leaves = await _db.LeaveRequests
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        return Ok(leaves.Select(ToDto));
    }

    /// <summary>
    /// GET api/Areas/Admin/Leaves/my
    /// Returns the current user's leave requests.
    /// </summary>
    [HttpGet("my")]
    public async Task<ActionResult> GetMy()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await _db.Users.FindAsync(userId);
        if (user?.EmployeeId == null)
            return BadRequest(new { message = "No employee profile linked to this account." });

        var leaves = await _db.LeaveRequests
            .Include(l => l.LeaveType)
            .Where(l => l.EmployeeId == user.EmployeeId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        return Ok(leaves.Select(ToDto));
    }

    /// <summary>
    /// POST api/Areas/Admin/Leaves
    /// Submit a new leave request.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<LeaveDto>> Create([FromBody] LeaveCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return Unauthorized(new { message = "User not found" });
        if (user.EmployeeId == null)
            return BadRequest(new { message = "No employee profile linked to this account." });

        var leaveType = await _db.LeaveTypes.FirstOrDefaultAsync(t => t.Id == dto.LeaveTypeId);
        if (leaveType == null)
            return BadRequest(new { message = "Leave type not found." });

        if (!DateTime.TryParse(dto.StartDate, out var startDate))
            return BadRequest(new { message = "Invalid start date." });
        if (!DateTime.TryParse(dto.EndDate, out var endDate))
            return BadRequest(new { message = "Invalid end date." });
        if (endDate < startDate)
            return BadRequest(new { message = "End date must be after start date." });

        var duration = _balanceService.WorkingDays(startDate, endDate);
        if (duration <= 0)
            return BadRequest(new { message = "No working days in the selected range (Friday/Saturday excluded)." });

        if (leaveType.IsPaid)
        {
            var remaining = await _balanceService.RemainingAsync(_db, user.EmployeeId.Value, leaveType.Id, startDate.Year);
            if (duration > remaining)
                return BadRequest(new
                {
                    message = $"Insufficient balance. You have {remaining} days remaining for this leave type but requested {duration}."
                });
        }

        var leave = new LeaveRequest
        {
            EmployeeId = user.EmployeeId.Value,
            LeaveTypeId = leaveType.Id,
            StartDate = startDate,
            EndDate = endDate,
            DurationDays = duration,
            Reason = dto.Reason,
            Status = "Pending"
        };

        _db.LeaveRequests.Add(leave);
        await _db.SaveChangesAsync();

        await _audit.LogAsync("Create", "LeaveRequest", leave.Id.ToString(),
            $"Leave request {duration} days ({leaveType.Name}) from {leave.StartDate:yyyy-MM-dd} to {leave.EndDate:yyyy-MM-dd}");

        return CreatedAtAction(nameof(GetAll), new { id = leave.Id }, ToDto(leave));
    }

    /// <summary>
    /// PUT api/Areas/Admin/Leaves/{id}/status
    /// Approve or reject a leave request (Admin/HR only). Approving deducts balance.
    /// </summary>
    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] LeaveStatusUpdateDto dto)
    {
        if (dto.Status != "Approved" && dto.Status != "Rejected")
            return BadRequest(new { message = "Status must be 'Approved' or 'Rejected'." });

        var leave = await _db.LeaveRequests
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leave == null)
            return NotFound(new { message = "Leave request not found" });

        if (leave.Status != "Pending")
            return BadRequest(new { message = "This request has already been processed." });

        if (dto.Status == "Approved" && leave.LeaveType != null && leave.LeaveType.IsPaid)
        {
            var remaining = await _balanceService.RemainingAsync(_db, leave.EmployeeId, leave.LeaveTypeId, leave.StartDate.Year);
            if (leave.DurationDays > remaining)
                return BadRequest(new
                {
                    message = $"Insufficient balance to approve. Remaining: {remaining} days, requested: {leave.DurationDays} days."
                });
        }

        var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        leave.Status = dto.Status;
        leave.ApprovedBy = adminId;
        leave.ApprovedAt = DateTime.UtcNow;
        leave.RejectionReason = dto.Status == "Rejected" ? dto.RejectionReason : null;

        // Deduct / restore leave balance on approval
        if (dto.Status == "Approved" && leave.LeaveType != null && leave.LeaveType.IsPaid)
        {
            var balance = await _balanceService.EnsureBalanceAsync(_db, leave.EmployeeId, leave.LeaveTypeId, leave.StartDate.Year);
            balance.UsedDays += leave.DurationDays;
        }

        await _db.SaveChangesAsync();

        await _audit.LogAsync(dto.Status == "Approved" ? "Approve" : "Reject", "LeaveRequest", leave.Id.ToString(),
            $"{dto.Status} {leave.DurationDays} days leave for employee {leave.Employee?.FullName}");

        return Ok(new { message = $"Leave request {dto.Status.ToLower()}.", status = dto.Status });
    }

    private static LeaveDto ToDto(LeaveRequest l) => new()
    {
        Id = l.Id,
        EmployeeId = l.EmployeeId,
        EmployeeName = l.Employee != null ? l.Employee.FullName : "Unknown",
        LeaveTypeId = l.LeaveTypeId,
        LeaveTypeName = l.LeaveType?.Name,
        LeaveTypeNameAr = l.LeaveType?.NameAr,
        StartDate = l.StartDate.ToString("yyyy-MM-dd"),
        EndDate = l.EndDate.ToString("yyyy-MM-dd"),
        DurationDays = l.DurationDays,
        Reason = l.Reason,
        Status = l.Status,
        RejectionReason = l.RejectionReason,
        CreatedAt = l.CreatedAt.ToString("yyyy-MM-dd HH:mm")
    };
}
