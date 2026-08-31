using HrSystem.Application.Exceptions;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Services;

public sealed class AuthService(IRepository<User> users, IPasswordHasher passwordHasher, ITokenService tokenService, IAuditService audit) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalized = request.Email.Trim().ToLowerInvariant();
        var user = (await users.QueryAsync(u => u, u => u.Email == normalized && u.IsActive, 0, 1, cancellationToken)).FirstOrDefault();
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash)) return null;
        var (token, expiresAt) = tokenService.Create(user);
        await audit.WriteAsync("Login", nameof(User), user.Id.ToString(), null, cancellationToken);
        return new(token, expiresAt, user.Id, user.FullName, user.Role, user.EmployeeId);
    }
}
