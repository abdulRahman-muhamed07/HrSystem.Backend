namespace HrSystem.Application.Abstractions.Security;

public interface ITokenRevocationService
{
    Task RevokeAsync(string jti, DateTimeOffset expiresAt, CancellationToken ct);
    Task<bool> IsRevokedAsync(string jti, CancellationToken ct);
}
