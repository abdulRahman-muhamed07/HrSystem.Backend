using HrSystem.Application;
using HrSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin,HR")]
[Route("api/employees")]
public sealed class EmployeesController(IEmployeeService service) : ControllerBase
{
    [HttpGet]
    public Task<PagedResult<EmployeeListItem>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, CancellationToken ct = default) => service.GetPagedAsync(page, pageSize, search, ct);
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDetails>> GetById(int id, CancellationToken ct) => (await service.GetAsync(id, ct)) is { } item ? Ok(item) : NotFound();
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateEmployeeRequest request, CancellationToken ct) { var id = await service.CreateAsync(request, ct); return CreatedAtAction(nameof(GetById), new { id }, id); }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateEmployeeRequest request, CancellationToken ct) { await service.UpdateAsync(id, request, ct); return NoContent(); }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) { await service.DeleteAsync(id, ct); return NoContent(); }
}
