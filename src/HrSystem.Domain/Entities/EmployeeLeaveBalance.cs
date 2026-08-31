namespace HrSystem.Domain.Entities;

public sealed class EmployeeLeaveBalance
{
    public int Id { get; private set; }
    public int EmployeeId { get; private set; }
    public int LeaveTypeId { get; private set; }
    public int Year { get; private set; }
    public decimal EntitledDays { get; private set; }
    public decimal UsedDays { get; private set; }
    public decimal AdjustedDays { get; private set; }
    public Employee? Employee { get; private set; }
    public LeaveType? LeaveType { get; private set; }
    private EmployeeLeaveBalance() { }
    public EmployeeLeaveBalance(int employeeId, int leaveTypeId, int year, decimal entitledDays)
    { EmployeeId = employeeId; LeaveTypeId = leaveTypeId; Year = year; EntitledDays = entitledDays; }
    public decimal AvailableDays => EntitledDays + AdjustedDays - UsedDays;
    public void AddUsage(decimal days) => UsedDays += days;
    public void Adjust(decimal days) => AdjustedDays += days;
}
