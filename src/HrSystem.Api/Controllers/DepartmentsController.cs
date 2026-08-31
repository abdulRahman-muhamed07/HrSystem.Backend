using HrSystem.Application;
using HrSystem.Application.Features.Departments;
using HrSystem.Application.Models.Departments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin,HR")]
[Route("api/departments")]
public sealed class DepartmentsController(DepartmentHandler handler) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<DepartmentDto>> GetAll(CancellationToken ct) => handler.GetAllAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentDto>> GetById(int id, CancellationToken ct)
        => (await handler.GetByIdAsync(id, ct)) is { } department ? Ok(department) : NotFound();

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateDepartmentRequest request, CancellationToken ct)
    {
        var id = await handler.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CreateDepartmentRequest request, CancellationToken ct)
    {
        await handler.UpdateAsync(id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await handler.DeleteAsync(id, ct);
        return NoContent();
    }
}
