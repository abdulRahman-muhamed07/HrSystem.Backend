namespace HrSystem.Application;

public sealed record LeaveTypeDto(int Id, string Name, string? NameAr, decimal DaysPerYear, bool IsPaid, bool IsActive, string? Description);
