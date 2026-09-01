using HrSystem.Application.Models.Authentication;

namespace HrSystem.Application.Features.Authentication.Contracts;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<LoginResponse?> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken);
    Task LogoutAsync(string jti, DateTimeOffset expiresAt, string? refreshToken, CancellationToken cancellationToken);
}
