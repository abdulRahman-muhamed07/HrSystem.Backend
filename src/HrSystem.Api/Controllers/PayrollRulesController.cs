using HrSystem.Application.Features.PayrollRules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin,HR")]
[Route("api/payroll-rules")]
public sealed class PayrollRulesController(PayrollRuleHandler handler) : ControllerBase
{
    [HttpGet] public Task<IReadOnlyCollection<PayrollRuleDto>> Get(CancellationToken ct) => handler.GetAsync(ct);
    [HttpPost] public async Task<ActionResult<int>> Create(CreatePayrollRuleRequest request, CancellationToken ct) => Ok(await handler.CreateAsync(request, ct));
}
