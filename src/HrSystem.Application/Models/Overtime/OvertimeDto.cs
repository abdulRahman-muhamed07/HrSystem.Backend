using HrSystem.Domain.Enums;

namespace HrSystem.Application.Models.Overtime;

public sealed record OvertimeDto(int Id, int EmployeeId, string EmployeeName, DateTime Date, decimal Hours, decimal RateMultiplier, string? Reason, OvertimeStatus Status);
