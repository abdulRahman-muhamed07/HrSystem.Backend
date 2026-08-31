using HrSystem.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize]
[Route("api/dashboard")]
public sealed class DashboardController(IDashboardService service) : ControllerBase
{
    [HttpGet("summary")]
    public Task<DashboardDto> Get([FromQuery] int year, [FromQuery] int month, CancellationToken ct) => service.GetAsync(year, month, ct);
}
