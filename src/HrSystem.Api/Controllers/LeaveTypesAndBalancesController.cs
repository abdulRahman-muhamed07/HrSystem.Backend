using HrSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize]
[Route("api/leave-types")]
public sealed class LeaveTypesController(ILeaveTypeService service) : ControllerBase
{
    [HttpGet] public Task<IReadOnlyCollection<LeaveTypeDto>> GetAll(CancellationToken ct) => service.GetAllAsync(ct);
}

[ApiController, Authorize]
[Route("api/leave-balances")]
public sealed class LeaveBalancesController(ILeaveBalanceReadService service) : ControllerBase
{
    [HttpGet("{employeeId:int}")]
    public Task<IReadOnlyCollection<LeaveBalanceDto>> Get(int employeeId, [FromQuery] int year, CancellationToken ct) => service.GetAsync(employeeId, year, ct);
}
