using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Data;
using HrSystem.Backend.Models.Dtos;

namespace HrSystem.Backend.Controllers;

[Route("api/Areas/Admin/AuditLogs")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AuditLogsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AuditLogsController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// GET api/Areas/Admin/AuditLogs?page=1&pageSize=50&action=Login&entity=User
    /// Query audit logs with pagination and filters.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] string? action, [FromQuery] string? entity,
        [FromQuery] int? userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 500);

        var query = _db.AuditLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(action)) query = query.Where(a => a.Action == action);
        if (!string.IsNullOrWhiteSpace(entity)) query = query.Where(a => a.EntityName == entity);
        if (userId.HasValue) query = query.Where(a => a.UserId == userId);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Items = items.Select(a => new AuditLogDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UserName = a.UserName,
                Action = a.Action,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Details = a.Details,
                IpAddress = a.IpAddress,
                Timestamp = a.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
            })
        });
    }

    /// <summary>
    /// GET api/Areas/Admin/AuditLogs/recent
    /// Latest 20 audit entries (for dashboard widget).
    /// </summary>
    [HttpGet("recent")]
    public async Task<ActionResult> Recent()
    {
        var items = await _db.AuditLogs
            .OrderByDescending(a => a.Timestamp)
            .Take(20)
            .ToListAsync();

        return Ok(items.Select(a => new AuditLogDto
        {
            Id = a.Id,
            UserId = a.UserId,
            UserName = a.UserName,
            Action = a.Action,
            EntityName = a.EntityName,
            EntityId = a.EntityId,
            Details = a.Details,
            IpAddress = a.IpAddress,
            Timestamp = a.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
        }));
    }
}
