namespace HrSystem.Application;

public interface IAuditService
{
    Task WriteAsync(string action, string entityName, string? entityId = null, string? details = null, CancellationToken cancellationToken = default);
}
