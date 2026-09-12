using HrSystem.Application.Features.Loans;
using HrSystem.Application.Models.Loans;
using HrSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize]
[Route("api/loans")]
public sealed class LoansController(LoanHandler handler) : ControllerBase
{
    [Authorize(Roles = "Admin,HR")]
    [HttpGet("pending")]
    public Task<IReadOnlyCollection<LoanDto>> GetPending(CancellationToken ct) => handler.GetPendingAsync(ct);

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateLoanRequest request, CancellationToken ct)
    {
        var id = await handler.CreateAsync(request, ct);
        return Created($"api/loans/{id}", id);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost("{id:int}/decision")]
    public async Task<IActionResult> Decide(int id, [FromQuery] bool approve, CancellationToken ct)
    {
        await handler.DecideAsync(id, approve, ct);
        return NoContent();
    }
}
