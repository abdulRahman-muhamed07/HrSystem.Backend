namespace HrSystem.Application;

public sealed record LeaveBalanceDto(int Id, int EmployeeId, int LeaveTypeId, string LeaveTypeName, int Year, decimal EntitledDays, decimal UsedDays, decimal AdjustedDays, decimal AvailableDays);
