using HrSystem.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin,HR")]
[Route("api/departments")]
public sealed class DepartmentsController(IDepartmentService service) : ControllerBase
{
    [HttpGet] public Task<IReadOnlyCollection<DepartmentDto>> GetAll(CancellationToken ct) => service.GetAllAsync(ct);
    [HttpPost] public async Task<ActionResult<int>> Create(CreateDepartmentRequest request, CancellationToken ct) { var id = await service.CreateAsync(request, ct); return Ok(id); }
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, CreateDepartmentRequest request, CancellationToken ct) { await service.UpdateAsync(id, request, ct); return NoContent(); }
}
