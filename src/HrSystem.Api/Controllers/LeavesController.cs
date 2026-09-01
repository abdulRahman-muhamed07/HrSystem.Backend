using HrSystem.Application;
using HrSystem.Application.Features.Leaves;
using HrSystem.Application.Models.Leaves;
using HrSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize]
[Route("api/leaves")]
public sealed class LeavesController(LeaveHandler handler) : ControllerBase
{
    [HttpGet] public Task<PagedResult<LeaveRequestDto>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] LeaveRequestStatus? status = null, CancellationToken ct = default) => handler.GetAsync(page, pageSize, status, ct);
    [HttpPost] public async Task<ActionResult<int>> Create(CreateLeaveRequest request, CancellationToken ct) { var id = await handler.CreateAsync(request, ct); return Ok(id); }
    [Authorize(Roles = "Admin,HR")]
    [HttpPost("{id:int}/decision")]
    public async Task<IActionResult> Decide(int id, LeaveDecisionRequest request, CancellationToken ct) { await handler.DecideAsync(id, request, ct); return NoContent(); }
}
