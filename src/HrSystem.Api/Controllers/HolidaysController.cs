using HrSystem.Application.Features.Holidays;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin,HR")]
[Route("api/holidays")]
public sealed class HolidaysController(HolidayHandler handler) : ControllerBase
{
    [HttpGet] public Task<IReadOnlyCollection<HolidayDto>> Get([FromQuery] int year, CancellationToken ct) => handler.GetAsync(year, ct);
    [HttpPost] public async Task<ActionResult<int>> Create(CreateHolidayRequest request, CancellationToken ct) => Ok(await handler.CreateAsync(request, ct));
}
