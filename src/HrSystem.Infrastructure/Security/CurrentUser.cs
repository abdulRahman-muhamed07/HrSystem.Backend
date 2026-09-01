using System.Security.Claims;
using HrSystem.Application;
using Microsoft.AspNetCore.Http;

namespace HrSystem.Infrastructure.Security;

internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal Principal => httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    public int? UserId => int.TryParse(Principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    public string? UserName => Principal.FindFirstValue(ClaimTypes.Name) ?? Principal.FindFirstValue("unique_name");
    public string? Role => Principal.FindFirstValue(ClaimTypes.Role) ?? Principal.FindFirstValue("role");
}
