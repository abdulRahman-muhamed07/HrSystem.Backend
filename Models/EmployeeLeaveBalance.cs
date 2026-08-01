namespace HrSystem.Backend.Models;

public class EmployeeLeaveBalance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public int Year { get; set; }
    public decimal EntitledDays { get; set; }   // Allowed for the year (prorated on hire)
    public decimal UsedDays { get; set; }
    public decimal AdjustedDays { get; set; }   // Admin manual adjustments (+/-)

    // Navigation
    public virtual Employee? Employee { get; set; }
    public virtual LeaveType? LeaveType { get; set; }
}
