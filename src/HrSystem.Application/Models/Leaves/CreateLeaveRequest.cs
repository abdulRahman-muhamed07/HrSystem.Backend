namespace HrSystem.Application;

public sealed record CreateLeaveRequest(int EmployeeId, int LeaveTypeId, DateTime StartDate, DateTime EndDate, string Reason);
