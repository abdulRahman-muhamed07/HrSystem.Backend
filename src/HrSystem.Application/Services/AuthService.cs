using HrSystem.Application.Exceptions;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Enums;

namespace HrSystem.Application.Services;

public sealed class AuthService(
    IRepository<User> users,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ITokenRevocationService tokenRevocation,
    IAuditService audit,
    IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var normalized = request.Email.Trim().ToLowerInvariant();
        var user = (await users.QueryAsync(
            u => u,
            u => u.Email == normalized && u.IsActive,
            0,
            1,
            ct)).FirstOrDefault();

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            return null;

        var (token, expiresAt) = tokenService.Create(user);
        await audit.WriteAsync("Login", nameof(User), user.Id.ToString(), null, ct);
        return new(token, expiresAt, user.Id, user.FullName, user.Role, user.EmployeeId);
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var fullName = request.FullName.Trim();
        var email = request.Email.Trim().ToLowerInvariant();
        var password = request.Password;

        if (string.IsNullOrWhiteSpace(fullName))
            throw new BusinessRuleException("Full name is required.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new BusinessRuleException("A valid email is required.");

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            throw new BusinessRuleException("Password must be at least 8 characters.");

        if (await users.CountAsync(u => u.Email == email, ct) > 0)
            throw new BusinessRuleException("An account with this email already exists.");

        var user = new User(
            email,
            passwordHasher.Hash(password),
            fullName,
            UserRole.Employee);

        await users.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var (token, expiresAt) = tokenService.Create(user);
        await audit.WriteAsync("Register", nameof(User), user.Id.ToString(), null, ct);

        return new(token, expiresAt, user.Id, user.FullName, user.Role, user.EmployeeId);
    }

    public async Task LogoutAsync(string jti, DateTimeOffset expiresAt, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(jti))
            throw new BusinessRuleException("Token identifier is required.");

        await tokenRevocation.RevokeAsync(jti, expiresAt, ct);
        await audit.WriteAsync("Logout", nameof(User), "current", null, ct);
    }
}
