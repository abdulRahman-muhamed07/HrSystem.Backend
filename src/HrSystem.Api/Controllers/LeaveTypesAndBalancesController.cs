using HrSystem.Application;
using HrSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin,HR")]
[Route("api/leave-types")]
public sealed class LeaveTypesController(ILeaveTypeService service) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<LeaveTypeDto>> GetAll(CancellationToken ct)
        => service.GetAllAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LeaveTypeDto>> GetById(int id, CancellationToken ct)
        => (await service.GetByIdAsync(id, ct)) is { } item ? Ok(item) : NotFound();

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateLeaveTypeRequest request, CancellationToken ct)
    {
        var id = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CreateLeaveTypeRequest request, CancellationToken ct)
    {
        await service.UpdateAsync(id, request, ct);
        return NoContent();
    }

    [HttpPatch("{id:int}/active")]
    public async Task<IActionResult> SetActive(int id, [FromQuery] bool isActive, CancellationToken ct)
    {
        await service.SetActiveAsync(id, isActive, ct);
        return NoContent();
    }
}

[ApiController, Authorize]
[Route("api/leave-balances")]
public sealed class LeaveBalancesController(ILeaveBalanceReadService service) : ControllerBase
{
    [HttpGet("{employeeId:int}")]
    public Task<IReadOnlyCollection<LeaveBalanceDto>> Get(int employeeId, [FromQuery] int year, CancellationToken ct)
        => service.GetAsync(employeeId, year, ct);
}
