namespace HrSystem.Backend.Models;

public class LeaveType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;           // Annual / Sick / Unpaid / Maternity ...
    public string? NameAr { get; set; }                        // Arabic label (إجازة سنوية ...)
    public decimal DaysPerYear { get; set; } = 21;             // Entitlement per year
    public bool IsPaid { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }

    // Navigation
    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public virtual ICollection<EmployeeLeaveBalance> Balances { get; set; } = new List<EmployeeLeaveBalance>();
}
