using HrSystem.Domain.Enums;

namespace HrSystem.Domain.Entities;

public sealed class EmployeeLoan
{
    public int Id { get; private set; }
    public int EmployeeId { get; private set; }
    public decimal Amount { get; private set; }
    public int Installments { get; private set; }
    public decimal MonthlyDeduction { get; private set; }
    public decimal RemainingAmount { get; private set; }
    public string? Reason { get; private set; }
    public LoanStatus Status { get; private set; } = LoanStatus.Pending;
    public int? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime RequestDate { get; private set; } = DateTime.UtcNow;
    public Employee? Employee { get; private set; }
    private EmployeeLoan() { }
    public EmployeeLoan(int employeeId, decimal amount, int installments, string? reason)
    { EmployeeId = employeeId; Amount = amount; Installments = installments; MonthlyDeduction = Math.Round(amount / installments, 2); RemainingAmount = amount; Reason = reason?.Trim(); }
    public void Approve(int userId) { Status = LoanStatus.Approved; ApprovedBy = userId; ApprovedAt = DateTime.UtcNow; }
    public void Reject(int userId) { Status = LoanStatus.Rejected; ApprovedBy = userId; ApprovedAt = DateTime.UtcNow; }
}
