using HrSystem.Backend.Data;
using HrSystem.Backend.Models;
using System.Security.Claims;

namespace HrSystem.Backend.Services;

/// <summary>
/// Writes audit log entries for security / compliance tracking.
/// </summary>
public interface IAuditService
{
    Task LogAsync(string action, string entityName, string? entityId, string? details = null);
}

public class AuditService : IAuditService
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _http;

    public AuditService(AppDbContext db, IHttpContextAccessor http)
    {
        _db = db;
        _http = http;
    }

    public async Task LogAsync(string action, string entityName, string? entityId, string? details = null)
    {
        var principal = _http.HttpContext?.User;
        var userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = principal?.FindFirstValue(ClaimTypes.Name);

        var log = new AuditLog
        {
            UserId = int.TryParse(userId, out var id) ? id : null,
            UserName = userName,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Details = details,
            IpAddress = _http.HttpContext?.Connection.RemoteIpAddress?.ToString(),
            Timestamp = DateTime.UtcNow
        };

        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }
}
