using HrSystem.Application.Features.Payroll;
using HrSystem.Application.Models.Payroll;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin,HR")]
[Route("api/payroll")]
public sealed class PayrollController(PayrollHandler handler) : ControllerBase
{
    [HttpPost("generate")]
    public Task<PayrollDto> Generate([FromQuery] int employeeId, [FromQuery] int year, [FromQuery] int month, CancellationToken ct) => handler.GenerateAsync(employeeId, year, month, ct);
    [HttpGet]
    public Task<IReadOnlyCollection<PayrollDto>> GetMonth([FromQuery] int year, [FromQuery] int month, CancellationToken ct) => handler.GetMonthAsync(year, month, ct);
    [HttpPost("{id:int}/pay")]
    public async Task<IActionResult> Pay(int id, CancellationToken ct) { await handler.PayAsync(id, ct); return NoContent(); }
}
