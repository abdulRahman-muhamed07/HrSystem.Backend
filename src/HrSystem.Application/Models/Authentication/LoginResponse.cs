using HrSystem.Domain.Enums;

namespace HrSystem.Application.Models.Authentication;

public sealed record LoginResponse(string Token, DateTime ExpiresAt, int UserId, string FullName, UserRole Role, int? EmployeeId);
