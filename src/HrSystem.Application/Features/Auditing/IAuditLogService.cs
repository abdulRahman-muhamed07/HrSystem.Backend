using HrSystem.Application.Models.Auditing;

namespace HrSystem.Application;

public interface IAuditLogService
{
    Task<IReadOnlyCollection<AuditLogDto>> GetRecentAsync(int take, CancellationToken ct);
}
