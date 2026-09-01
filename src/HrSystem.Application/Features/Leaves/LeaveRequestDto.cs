using HrSystem.Domain.Enums;

namespace HrSystem.Application.Models.Leaves;

public sealed record LeaveRequestDto(int Id, int EmployeeId, string EmployeeName, int LeaveTypeId, string LeaveTypeName, DateTime StartDate, DateTime EndDate, decimal DurationDays, string Reason, LeaveRequestStatus Status, string? RejectionReason);
