using Microsoft.IdentityModel.Tokens;

namespace HrSystem.Backend.Services;

public record JwtSettings(string Key, string Issuer, string Audience, int ExpirationInMinutes);

public interface IJwtService
{
    string GenerateToken(int userId, string email, string fullName, string role, int? employeeId = null);
}

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(JwtSettings settings)
    {
        _settings = settings;
    }

    public string GenerateToken(int userId, string email, string fullName, string role, int? employeeId = null)
    {
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<System.Security.Claims.Claim>
        {
            new(System.Security.Claims.ClaimTypes.NameIdentifier, userId.ToString()),
            new(System.Security.Claims.ClaimTypes.Email, email),
            new(System.Security.Claims.ClaimTypes.Name, fullName),
            new(System.Security.Claims.ClaimTypes.Role, role)
        };

        if (employeeId.HasValue)
            claims.Add(new("EmployeeId", employeeId.Value.ToString()));

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationInMinutes),
            signingCredentials: credentials
        );

        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }
}