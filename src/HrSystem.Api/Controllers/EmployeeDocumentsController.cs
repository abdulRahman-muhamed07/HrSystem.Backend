using HrSystem.Application.Features.EmployeeDocuments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin,HR")]
[Route("api/employee-documents")]
public sealed class EmployeeDocumentsController(EmployeeDocumentHandler handler) : ControllerBase
{
    [HttpGet("employee/{employeeId:int}")]
    public Task<IReadOnlyCollection<EmployeeDocumentDto>> Get(int employeeId, CancellationToken ct) => handler.GetAsync(employeeId, ct);

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateEmployeeDocumentRequest request, CancellationToken ct) => Ok(await handler.CreateAsync(request, ct));
}
