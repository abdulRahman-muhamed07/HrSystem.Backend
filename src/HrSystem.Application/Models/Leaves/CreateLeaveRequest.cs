namespace HrSystem.Application.Models.Leaves;

public sealed record CreateLeaveRequest(int EmployeeId, int LeaveTypeId, DateTime StartDate, DateTime EndDate, string Reason);
