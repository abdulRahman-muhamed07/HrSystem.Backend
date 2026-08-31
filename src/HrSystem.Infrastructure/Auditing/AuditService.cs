using HrSystem.Application;
using HrSystem.Domain.Entities;

namespace HrSystem.Infrastructure.Auditing;

public sealed class AuditService(IRepository<AuditLog> logs, IUnitOfWork unitOfWork, ICurrentUser currentUser) : IAuditService
{
    public async Task WriteAsync(string action, string entityName, string? entityId = null, string? details = null, CancellationToken cancellationToken = default)
    {
        await logs.AddAsync(new AuditLog(currentUser.UserId, currentUser.UserName, action, entityName, entityId, details, null), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
