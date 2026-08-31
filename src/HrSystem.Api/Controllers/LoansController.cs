using HrSystem.Application.Features.Loans;
using HrSystem.Application.Models.Loans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize]
[Route("api/loans")]
public sealed class LoansController(LoanHandler handler) : ControllerBase
{
    [HttpGet("pending")] public Task<IReadOnlyCollection<LoanDto>> GetPending(CancellationToken ct) => handler.GetPendingAsync(ct);
    [HttpPost] public async Task<ActionResult<int>> Create(CreateLoanRequest request, CancellationToken ct) { var id = await handler.CreateAsync(request, ct); return Ok(id); }
    [Authorize(Roles = "Admin,HR")]
    [HttpPost("{id:int}/decision")]
    public async Task<IActionResult> Decide(int id, [FromQuery] bool approve, CancellationToken ct) { await handler.DecideAsync(id, approve, ct); return NoContent(); }
}
