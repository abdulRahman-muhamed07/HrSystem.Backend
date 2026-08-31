using HrSystem.Application.Features.Dashboard;
using HrSystem.Application.Models.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize]
[Route("api/dashboard")]
public sealed class DashboardController(DashboardHandler handler) : ControllerBase
{
    [HttpGet("summary")]
    public Task<DashboardDto> Get([FromQuery] int year, [FromQuery] int month, CancellationToken ct) => handler.GetAsync(year, month, ct);
}
