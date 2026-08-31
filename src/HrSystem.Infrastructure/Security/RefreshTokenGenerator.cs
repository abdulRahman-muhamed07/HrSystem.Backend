using System.Security.Cryptography;
using System.Text;
using HrSystem.Application;
using Microsoft.Extensions.Configuration;

namespace HrSystem.Infrastructure.Security;

public sealed class RefreshTokenGenerator(IConfiguration configuration) : IRefreshTokenGenerator
{
    public (string RawToken, string TokenHash, DateTime ExpiresAt) Generate()
    {
        var days = configuration.GetValue("Jwt:RefreshTokenExpirationDays", 7);
        var raw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return (raw, Hash(raw), DateTime.UtcNow.AddDays(days));
    }

    public string Hash(string rawToken)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));
}
