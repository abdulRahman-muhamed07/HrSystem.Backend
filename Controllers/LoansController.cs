using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Data;
using HrSystem.Backend.Models;
using HrSystem.Backend.Models.Dtos;
using HrSystem.Backend.Services;
using System.Security.Claims;

namespace HrSystem.Backend.Controllers;

[Route("api/Areas/Admin/Loans")]
[ApiController]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;

    public LoansController(AppDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    /// <summary>
    /// GET api/Areas/Admin/Loans
    /// All loan/advance requests. Optional ?status= filter.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] string? status)
    {
        var query = _db.EmployeeLoans
            .Include(l => l.Employee)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(l => l.Status == status);

        var items = await query
            .OrderByDescending(l => l.RequestDate)
            .ToListAsync();

        return Ok(items.Select(ToDto));
    }

    /// <summary>
    /// GET api/Areas/Admin/Loans/my
    /// Current user's loans.
    /// </summary>
    [HttpGet("my")]
    public async Task<ActionResult> GetMy()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await _db.Users.FindAsync(userId);
        if (user?.EmployeeId == null)
            return BadRequest(new { message = "No employee profile linked to this account." });

        var items = await _db.EmployeeLoans
            .Where(l => l.EmployeeId == user.EmployeeId)
            .OrderByDescending(l => l.RequestDate)
            .ToListAsync();

        return Ok(items.Select(ToDto));
    }

    /// <summary>
    /// POST api/Areas/Admin/Loans
    /// Request a salary advance/loan.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<LoanDto>> Create([FromBody] LoanCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return Unauthorized(new { message = "User not found" });
        if (user.EmployeeId == null)
            return BadRequest(new { message = "No employee profile linked to this account." });

        if (dto.Amount <= 0)
            return BadRequest(new { message = "Amount must be greater than zero." });
        if (dto.Installments <= 0)
            return BadRequest(new { message = "Installments must be greater than zero." });

        var loan = new EmployeeLoan
        {
            EmployeeId = user.EmployeeId.Value,
            Amount = dto.Amount,
            Installments = dto.Installments,
            MonthlyDeduction = Math.Round(dto.Amount / dto.Installments, 2),
            RemainingAmount = dto.Amount,
            Reason = dto.Reason,
            Status = "Pending"
        };

        _db.EmployeeLoans.Add(loan);
        await _db.SaveChangesAsync();

        await _db.Entry(loan).Reference(l => l.Employee).LoadAsync();

        await _audit.LogAsync("Create", "EmployeeLoan", loan.Id.ToString(),
            $"Loan request of {dto.Amount} EGP over {dto.Installments} installments");

        return Ok(ToDto(loan));
    }

    /// <summary>
    /// PUT api/Areas/Admin/Loans/{id}/status
    /// Approve/reject a loan (Admin/HR).
    /// </summary>
    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] LoanStatusDto dto)
    {
        if (dto.Status != "Approved" && dto.Status != "Rejected")
            return BadRequest(new { message = "Status must be 'Approved' or 'Rejected'." });

        var loan = await _db.EmployeeLoans
            .Include(l => l.Employee)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null)
            return NotFound(new { message = "Loan not found" });

        if (loan.Status != "Pending")
            return BadRequest(new { message = "This request has already been processed." });

        var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        loan.Status = dto.Status;
        loan.ApprovedBy = adminId;
        loan.ApprovedAt = DateTime.UtcNow;
        if (dto.Status == "Rejected")
            loan.RemainingAmount = 0;

        await _db.SaveChangesAsync();

        await _audit.LogAsync(dto.Status == "Approved" ? "Approve" : "Reject", "EmployeeLoan", loan.Id.ToString(),
            $"{dto.Status} loan of {loan.Amount} EGP for {loan.Employee?.FullName}");

        return Ok(new { message = $"Loan {dto.Status.ToLower()}." });
    }

    /// <summary>
    /// DELETE api/Areas/Admin/Loans/{id}
    /// Delete a pending loan request.
    /// </summary>
    [Authorize(Roles = "Admin,HR")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var loan = await _db.EmployeeLoans.FindAsync(id);
        if (loan == null)
            return NotFound(new { message = "Loan not found" });

        if (loan.Status == "Approved")
            return BadRequest(new { message = "Cannot delete an approved loan. Mark it completed instead." });

        _db.EmployeeLoans.Remove(loan);
        await _db.SaveChangesAsync();

        await _audit.LogAsync("Delete", "EmployeeLoan", id.ToString(), "Deleted loan request");

        return NoContent();
    }

    /// <summary>
    /// PUT api/Areas/Admin/Loans/{id}/complete
    /// Mark an approved loan as fully repaid.
    /// </summary>
    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        var loan = await _db.EmployeeLoans.FindAsync(id);
        if (loan == null)
            return NotFound(new { message = "Loan not found" });

        if (loan.Status != "Approved")
            return BadRequest(new { message = "Only approved loans can be completed." });

        loan.Status = "Completed";
        loan.RemainingAmount = 0;
        await _db.SaveChangesAsync();

        await _audit.LogAsync("Complete", "EmployeeLoan", loan.Id.ToString(), $"Marked loan {loan.Id} as completed");

        return Ok(new { message = "Loan marked as completed." });
    }

    private static LoanDto ToDto(EmployeeLoan l) => new()
    {
        Id = l.Id,
        EmployeeId = l.EmployeeId,
        EmployeeName = l.Employee?.FullName ?? "Unknown",
        Amount = l.Amount,
        Installments = l.Installments,
        MonthlyDeduction = l.MonthlyDeduction,
        RemainingAmount = l.RemainingAmount,
        Reason = l.Reason,
        Status = l.Status,
        RequestDate = l.RequestDate.ToString("yyyy-MM-dd")
    };
}
