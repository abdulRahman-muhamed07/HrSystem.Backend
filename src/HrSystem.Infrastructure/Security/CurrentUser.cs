using System.Security.Claims;
using HrSystem.Application.Abstractions.Security;
using Microsoft.AspNetCore.Http;

namespace HrSystem.Infrastructure.Security;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public int? UserId =>
        int.TryParse(
            accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? accessor.HttpContext?.User.FindFirstValue("sub"),
            out var id) ? id : null;

    public string? UserName => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
    public string? Role => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
}