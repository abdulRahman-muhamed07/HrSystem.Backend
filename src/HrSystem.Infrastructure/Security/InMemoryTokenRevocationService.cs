using HrSystem.Application;
using Microsoft.Extensions.Caching.Memory;

namespace HrSystem.Infrastructure.Security;

public sealed class InMemoryTokenRevocationService(IMemoryCache cache) : ITokenRevocationService
{
    private static string Key(string jti) => $"revoked-jti:{jti}";

    public Task RevokeAsync(string jti, DateTimeOffset expiresAt, CancellationToken ct)
    {
        var lifetime = expiresAt - DateTimeOffset.UtcNow;
        if (lifetime <= TimeSpan.Zero)
            return Task.CompletedTask;

        cache.Set(Key(jti), true, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = lifetime
        });

        return Task.CompletedTask;
    }

    public Task<bool> IsRevokedAsync(string jti, CancellationToken ct)
        => Task.FromResult(cache.TryGetValue(Key(jti), out _));
}
