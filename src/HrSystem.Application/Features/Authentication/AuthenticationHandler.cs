using HrSystem.Application;
using HrSystem.Application.Models.Authentication;

namespace HrSystem.Application.Features.Authentication;

public sealed class AuthenticationHandler(IAuthService authService)
{
    public Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken ct) =>
        authService.RegisterAsync(request, ct);

    public Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct) =>
        authService.LoginAsync(request, ct);

    public Task<LoginResponse?> RefreshAsync(RefreshTokenRequest request, CancellationToken ct) =>
        authService.RefreshAsync(request, ct);

    public Task LogoutAsync(string jti, DateTimeOffset expiresAt, string? refreshToken, CancellationToken ct) =>
        authService.LogoutAsync(jti, expiresAt, refreshToken, ct);
}
