namespace HrSystem.Application.Models.Auditing;

public sealed record AuditLogDto(int Id, int? UserId, string? UserName, string Action, string EntityName, string? EntityId, string? Details, DateTime Timestamp);
