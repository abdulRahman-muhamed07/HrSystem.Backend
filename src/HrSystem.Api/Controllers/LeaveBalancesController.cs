using HrSystem.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize]
[Route("api/leave-balances")]
public sealed class LeaveBalancesController(ILeaveBalanceReadService service) : ControllerBase
{
    [HttpGet("{employeeId:int}")]
    public Task<IReadOnlyCollection<LeaveBalanceDto>> Get(int employeeId, [FromQuery] int year, CancellationToken ct)
        => service.GetAsync(employeeId, year, ct);
}
