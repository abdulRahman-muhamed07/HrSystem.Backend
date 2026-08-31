namespace HrSystem.Domain.Entities;

public sealed class AuditLog
{
    public int Id { get; private set; }
    public int? UserId { get; private set; }
    public string? UserName { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string EntityName { get; private set; } = string.Empty;
    public string? EntityId { get; private set; }
    public string? Details { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
    private AuditLog() { }
    public AuditLog(int? userId, string? userName, string action, string entityName, string? entityId, string? details, string? ipAddress)
    { UserId = userId; UserName = userName; Action = action; EntityName = entityName; EntityId = entityId; Details = details; IpAddress = ipAddress; }
}
