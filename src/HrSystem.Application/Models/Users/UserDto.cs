using HrSystem.Domain.Enums;

namespace HrSystem.Application;

public sealed record UserDto(int Id, string Email, string FullName, UserRole Role, bool IsActive, int? EmployeeId);
