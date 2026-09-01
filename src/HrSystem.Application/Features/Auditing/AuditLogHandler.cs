using HrSystem.Application;
using HrSystem.Application.Models.Auditing;

namespace HrSystem.Application.Features.Auditing;

public sealed class AuditLogHandler(IAuditLogService service)
{
    public Task<IReadOnlyCollection<AuditLogDto>> GetRecentAsync(int take, CancellationToken ct) => service.GetRecentAsync(take, ct);
}
