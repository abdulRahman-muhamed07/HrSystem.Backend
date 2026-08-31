using HrSystem.Application.Features.LeaveBalances;
using HrSystem.Application.Models.Leaves;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize]
[Route("api/leave-balances")]
public sealed class LeaveBalancesController(LeaveBalanceHandler handler) : ControllerBase
{
    [HttpGet("{employeeId:int}")]
    public Task<IReadOnlyCollection<LeaveBalanceDto>> Get(int employeeId, [FromQuery] int year, CancellationToken ct)
        => handler.GetAsync(employeeId, year, ct);
}
