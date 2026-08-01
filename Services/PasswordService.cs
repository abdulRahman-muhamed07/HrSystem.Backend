using System.Security.Cryptography;
using System.Text;

namespace HrSystem.Backend.Services;

/// <summary>
/// Password hashing built on HMAC-SHA256 with a static salt.
/// NOTE: for production, migrate to ASP.NET Core Identity or BCrypt.
/// </summary>
public interface IPasswordService
{
    string Hash(string password);
    bool Verify(string password, string storedHash);
}

public class PasswordService : IPasswordService
{
    private static readonly byte[] Salt = "HrSystem_2024_Salt"u8.ToArray();

    public string Hash(string password)
    {
        using var hmac = new HMACSHA256(Salt);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }

    public bool Verify(string password, string storedHash)
    {
        var computed = Hash(password);
        return computed == storedHash;
    }
}
