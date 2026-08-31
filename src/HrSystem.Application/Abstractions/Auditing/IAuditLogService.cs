namespace HrSystem.Application;

public interface IAuditLogService
{
    Task<IReadOnlyCollection<AuditLogDto>> GetRecentAsync(int take, CancellationToken ct);
}
