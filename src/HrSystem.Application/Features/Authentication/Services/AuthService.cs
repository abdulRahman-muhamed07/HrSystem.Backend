using FluentValidation;
using HrSystem.Application.Exceptions;
using HrSystem.Application.Models.Authentication;
using HrSystem.Application.Validation;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Enums;

namespace HrSystem.Application.Services;

public sealed class AuthService(
    IRepository<User> users,
    IRepository<RefreshToken> refreshTokens,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IRefreshTokenGenerator refreshTokenGenerator,
    ITokenRevocationService tokenRevocation,
    IAuditService audit,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IValidator<LoginRequest> loginValidator,
    IValidator<RegisterRequest> registerValidator) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        await loginValidator.ValidateApplicationRequestAsync(request, ct);
        var normalized = request.Email.Trim().ToLowerInvariant();
        var user = (await users.QueryAsync(u => u, u => u.Email == normalized && u.IsActive, 0, 1, ct)).FirstOrDefault();
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash)) return null;

        return await unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            var (accessToken, accessExpiresAt) = tokenService.Create(user);
            var refresh = refreshTokenGenerator.Generate();
            await refreshTokens.AddAsync(new RefreshToken(user.Id, refresh.TokenHash, refresh.ExpiresAt), token);
            await unitOfWork.SaveChangesAsync(token);
            await audit.WriteAsync("Login", nameof(User), user.Id.ToString(), null, token);
            return new LoginResponse(accessToken, accessExpiresAt, refresh.RawToken, refresh.ExpiresAt, user.Id, user.FullName, user.Role, user.EmployeeId);
        }, ct);
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        await registerValidator.ValidateApplicationRequestAsync(request, ct);
        var fullName = request.FullName.Trim();
        var email = request.Email.Trim().ToLowerInvariant();
        if (await users.CountAsync(u => u.Email == email, ct) > 0)
            throw new BusinessRuleException("An account with this email already exists.");

        return await unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            var user = new User(email, passwordHasher.Hash(request.Password), fullName, UserRole.Employee);
            await users.AddAsync(user, token);
            await unitOfWork.SaveChangesAsync(token);
            var (accessToken, accessExpiresAt) = tokenService.Create(user);
            var refresh = refreshTokenGenerator.Generate();
            await refreshTokens.AddAsync(new RefreshToken(user.Id, refresh.TokenHash, refresh.ExpiresAt), token);
            await unitOfWork.SaveChangesAsync(token);
            await audit.WriteAsync("Register", nameof(User), user.Id.ToString(), null, token);
            return new LoginResponse(accessToken, accessExpiresAt, refresh.RawToken, refresh.ExpiresAt, user.Id, user.FullName, user.Role, user.EmployeeId);
        }, ct);
    }

    public async Task<LoginResponse?> RefreshAsync(RefreshTokenRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken)) return null;
        var hash = refreshTokenGenerator.Hash(request.RefreshToken);
        var snapshot = (await refreshTokens.QueryAsync(r => r, r => r.TokenHash == hash && r.RevokedAt == null && r.ExpiresAt > DateTime.UtcNow, 0, 1, ct)).FirstOrDefault();
        if (snapshot is null) return null;

        var user = await users.GetByIdAsync(snapshot.UserId, ct);
        if (user is null || !user.IsActive) return null;

        return await unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            var current = await refreshTokens.GetByIdAsync(snapshot.Id, token);
            if (current is null || !current.IsActive || current.TokenHash != hash) return null;
            var next = refreshTokenGenerator.Generate();
            current.Revoke(next.TokenHash);
            await refreshTokens.AddAsync(new RefreshToken(user.Id, next.TokenHash, next.ExpiresAt), token);
            var (accessToken, accessExpiresAt) = tokenService.Create(user);
            await unitOfWork.SaveChangesAsync(token);
            await audit.WriteAsync("RefreshToken", nameof(User), user.Id.ToString(), null, token);
            return new LoginResponse(accessToken, accessExpiresAt, next.RawToken, next.ExpiresAt, user.Id, user.FullName, user.Role, user.EmployeeId);
        }, ct);
    }

    public async Task LogoutAsync(string jti, DateTimeOffset expiresAt, string? refreshToken, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(jti)) throw new BusinessRuleException("Token identifier is required.");
        await tokenRevocation.RevokeAsync(jti, expiresAt, ct);

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            var hash = refreshTokenGenerator.Hash(refreshToken);
            var snapshot = (await refreshTokens.QueryAsync(r => r, r => r.TokenHash == hash && r.RevokedAt == null, 0, 1, ct)).FirstOrDefault();
            if (snapshot is not null)
            {
                var stored = await refreshTokens.GetByIdAsync(snapshot.Id, ct);
                stored?.Revoke();
                if (stored is not null) await unitOfWork.SaveChangesAsync(ct);
            }
        }

        await audit.WriteAsync("Logout", nameof(User), currentUser.UserId?.ToString() ?? "unknown", null, ct);
    }
}
