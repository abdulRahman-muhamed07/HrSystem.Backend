using HrSystem.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize]
[Route("api/loans")]
public sealed class LoansController(ILoanService service) : ControllerBase
{
    [HttpGet("pending")] public Task<IReadOnlyCollection<LoanDto>> GetPending(CancellationToken ct) => service.GetPendingAsync(ct);
    [HttpPost] public async Task<ActionResult<int>> Create(CreateLoanRequest request, CancellationToken ct) { var id = await service.CreateAsync(request, ct); return Ok(id); }
    [Authorize(Roles = "Admin,HR")]
    [HttpPost("{id:int}/decision")]
    public async Task<IActionResult> Decide(int id, [FromQuery] bool approve, CancellationToken ct) { await service.DecideAsync(id, approve, ct); return NoContent(); }
}
