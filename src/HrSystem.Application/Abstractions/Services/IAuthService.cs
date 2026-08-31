namespace HrSystem.Application;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task LogoutAsync(string jti, DateTimeOffset expiresAt, CancellationToken cancellationToken);
}
