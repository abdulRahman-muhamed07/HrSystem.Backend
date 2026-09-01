using HrSystem.Application.Models.Auditing;

namespace HrSystem.Application.Features.Auditing.Contracts;

public interface IAuditLogService
{
    Task<IReadOnlyCollection<AuditLogDto>> GetRecentAsync(int take, CancellationToken ct);
}
