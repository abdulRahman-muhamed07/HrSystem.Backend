using HrSystem.Application.Features.Leaves;
using HrSystem.Application.Models.Leaves;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin,HR")]
[Route("api/leave-types")]
public sealed class LeaveTypesController(LeaveTypeHandler handler) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<LeaveTypeDto>> GetAll(CancellationToken ct) => handler.GetAllAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LeaveTypeDto>> GetById(int id, CancellationToken ct)
        => (await handler.GetByIdAsync(id, ct)) is { } item ? Ok(item) : NotFound();

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateLeaveTypeRequest request, CancellationToken ct)
    {
        var id = await handler.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CreateLeaveTypeRequest request, CancellationToken ct)
    {
        await handler.UpdateAsync(id, request, ct);
        return NoContent();
    }

    [HttpPatch("{id:int}/active")]
    public async Task<IActionResult> SetActive(int id, [FromQuery] bool isActive, CancellationToken ct)
    {
        await handler.SetActiveAsync(id, isActive, ct);
        return NoContent();
    }
}
