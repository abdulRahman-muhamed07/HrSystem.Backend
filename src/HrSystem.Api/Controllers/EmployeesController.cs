using HrSystem.Application;
using HrSystem.Application.Features.Employees;
using HrSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin,HR")]
[Route("api/employees")]
public sealed class EmployeesController(EmployeeHandler handler) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<EmployeeListItem>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, CancellationToken ct = default) => handler.GetAsync(page, pageSize, search, ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDetails>> GetById(int id, CancellationToken ct) => (await handler.GetByIdAsync(id, ct)) is { } item ? Ok(item) : NotFound();

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateEmployeeRequest request, CancellationToken ct) { var id = await handler.CreateAsync(request, ct); return CreatedAtAction(nameof(GetById), new { id }, id); }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateEmployeeRequest request, CancellationToken ct) { await handler.UpdateAsync(id, request, ct); return NoContent(); }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) { await handler.DeleteAsync(id, ct); return NoContent(); }
}
