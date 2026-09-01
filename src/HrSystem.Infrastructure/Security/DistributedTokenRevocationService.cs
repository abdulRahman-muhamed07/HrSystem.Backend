using HrSystem.Application;
using Microsoft.Extensions.Caching.Distributed;

namespace HrSystem.Infrastructure.Security;

public sealed class DistributedTokenRevocationService(IDistributedCache cache) : ITokenRevocationService
{
    private static string Key(string jti) => $"revoked-jti:{jti}";

    public async Task RevokeAsync(string jti, DateTimeOffset expiresAt, CancellationToken ct)
    {
        var lifetime = expiresAt - DateTimeOffset.UtcNow;
        if (lifetime <= TimeSpan.Zero)
            return;

        await cache.SetStringAsync(
            Key(jti),
            "1",
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = lifetime
            },
            ct);
    }

    public async Task<bool> IsRevokedAsync(string jti, CancellationToken ct)
        => await cache.GetStringAsync(Key(jti), ct) is not null;
}
