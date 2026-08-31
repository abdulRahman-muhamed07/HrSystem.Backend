namespace HrSystem.Application;

public sealed record CreateOvertimeRequest(int EmployeeId, DateTime Date, decimal Hours, decimal RateMultiplier = 1.25m, string? Reason = null);
