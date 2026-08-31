using HrSystem.Application;
using HrSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize]
[Route("api/leaves")]
public sealed class LeavesController(ILeaveService service) : ControllerBase
{
    [HttpGet] public Task<PagedResult<LeaveRequestDto>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] LeaveRequestStatus? status = null, CancellationToken ct = default) => service.GetPagedAsync(page, pageSize, status, ct);
    [HttpPost] public async Task<ActionResult<int>> Create(CreateLeaveRequest request, CancellationToken ct) { var id = await service.CreateAsync(request, ct); return Ok(id); }
    [Authorize(Roles = "Admin,HR")]
    [HttpPost("{id:int}/decision")]
    public async Task<IActionResult> Decide(int id, LeaveDecisionRequest request, CancellationToken ct) { await service.DecideAsync(id, request, ct); return NoContent(); }
}
