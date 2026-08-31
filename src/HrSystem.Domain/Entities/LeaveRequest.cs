using HrSystem.Domain.Enums;

namespace HrSystem.Domain.Entities;

public sealed class LeaveRequest : IConcurrencyTracked
{
    public int Id { get; private set; }
    public Guid Version { get; private set; } = Guid.NewGuid();
    public int EmployeeId { get; private set; }
    public int LeaveTypeId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal DurationDays { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public LeaveRequestStatus Status { get; private set; } = LeaveRequestStatus.Pending;
    public string? RejectionReason { get; private set; }
    public int? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Employee? Employee { get; private set; }
    public LeaveType? LeaveType { get; private set; }

    private LeaveRequest() { }
    public LeaveRequest(int employeeId, int leaveTypeId, DateTime startDate, DateTime endDate, decimal durationDays, string reason)
    {
        EmployeeId = employeeId; LeaveTypeId = leaveTypeId; StartDate = startDate.Date; EndDate = endDate.Date;
        DurationDays = durationDays; Reason = reason.Trim();
    }
    public void Approve(int approvedBy) { Status = LeaveRequestStatus.Approved; ApprovedBy = approvedBy; ApprovedAt = DateTime.UtcNow; RejectionReason = null; }
    public void Reject(int approvedBy, string reason) { Status = LeaveRequestStatus.Rejected; ApprovedBy = approvedBy; ApprovedAt = DateTime.UtcNow; RejectionReason = reason.Trim(); }
}
