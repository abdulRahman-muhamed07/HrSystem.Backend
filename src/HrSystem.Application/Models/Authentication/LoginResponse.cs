using HrSystem.Domain.Enums;

namespace HrSystem.Application.Models.Authentication;

public sealed record LoginResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt,
    int UserId,
    string FullName,
    UserRole Role,
    int? EmployeeId);
