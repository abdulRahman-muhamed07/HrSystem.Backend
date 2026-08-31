using HrSystem.Application.Models.Auditing;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Services;

public sealed class AuditLogService(IRepository<AuditLog> logs) : IAuditLogService
{
    public async Task<IReadOnlyCollection<AuditLogDto>> GetRecentAsync(int take, CancellationToken ct)
        => await logs.QueryAsync(
            l => new AuditLogDto(l.Id, l.UserId, l.UserName, l.Action, l.EntityName, l.EntityId, l.Details, l.Timestamp),
            null,
            0,
            Math.Clamp(take, 1, 200),
            ct);
}
