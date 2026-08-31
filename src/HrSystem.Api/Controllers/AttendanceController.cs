using HrSystem.Application;
using HrSystem.Application.Features.Attendance;
using HrSystem.Application.Models.Attendance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize]
[Route("api/attendance")]
public sealed class AttendanceController(AttendanceHandler handler) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<AttendanceDto>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] int? employeeId = null, CancellationToken ct = default) => handler.GetAsync(page, pageSize, employeeId, ct);
    [HttpPost("check-in")] public async Task<ActionResult<AttendanceDto>> CheckIn(CheckInRequest request, CancellationToken ct) => Ok(await handler.CheckInAsync(request, ct));
    [HttpPost("{id:int}/check-out")] public async Task<ActionResult<AttendanceDto>> CheckOut(int id, CheckOutRequest request, CancellationToken ct) => Ok(await handler.CheckOutAsync(id, request, ct));
}
