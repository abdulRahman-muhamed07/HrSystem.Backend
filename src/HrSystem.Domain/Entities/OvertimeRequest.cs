using HrSystem.Domain.Enums;

namespace HrSystem.Domain.Entities;

public sealed class OvertimeRequest
{
    public int Id { get; private set; }
    public int EmployeeId { get; private set; }
    public DateTime Date { get; private set; }
    public decimal Hours { get; private set; }
    public decimal RateMultiplier { get; private set; } = 1.25m;
    public string? Reason { get; private set; }
    public OvertimeStatus Status { get; private set; } = OvertimeStatus.Pending;
    public int? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Employee? Employee { get; private set; }
    private OvertimeRequest() { }
    public OvertimeRequest(int employeeId, DateTime date, decimal hours, decimal rateMultiplier, string? reason)
    { EmployeeId = employeeId; Date = date.Date; Hours = hours; RateMultiplier = rateMultiplier; Reason = reason?.Trim(); }
    public void Approve(int userId) { Status = OvertimeStatus.Approved; ApprovedBy = userId; ApprovedAt = DateTime.UtcNow; }
    public void Reject(int userId) { Status = OvertimeStatus.Rejected; ApprovedBy = userId; ApprovedAt = DateTime.UtcNow; }
}
