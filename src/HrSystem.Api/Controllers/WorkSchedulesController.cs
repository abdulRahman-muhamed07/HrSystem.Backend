using HrSystem.Application.Features.WorkSchedules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin,HR")]
[Route("api/work-schedules")]
public sealed class WorkSchedulesController(WorkScheduleHandler handler) : ControllerBase
{
    [HttpGet] public Task<IReadOnlyCollection<WorkScheduleDto>> Get(CancellationToken ct) => handler.GetAsync(ct);
    [HttpPost] public async Task<ActionResult<int>> Create(CreateWorkScheduleRequest request, CancellationToken ct) => Ok(await handler.CreateAsync(request, ct));
}
