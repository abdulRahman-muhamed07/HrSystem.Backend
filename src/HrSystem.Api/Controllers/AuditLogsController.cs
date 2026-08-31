using HrSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin,HR")]
[Route("api/audit-logs")]
public sealed class AuditLogsController(IAuditLogService service) : ControllerBase
{
    [HttpGet("recent")]
    public Task<IReadOnlyCollection<AuditLogDto>> GetRecent([FromQuery] int take = 50, CancellationToken ct = default) => service.GetRecentAsync(take, ct);
}
