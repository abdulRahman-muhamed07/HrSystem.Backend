namespace HrSystem.Domain.Entities;

public sealed class LeaveType
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? NameAr { get; private set; }
    public decimal DaysPerYear { get; private set; } = 21m;
    public bool IsPaid { get; private set; } = true;
    public bool IsActive { get; private set; } = true;
    public string? Description { get; private set; }
    public ICollection<LeaveRequest> LeaveRequests { get; private set; } = new List<LeaveRequest>();
    public ICollection<EmployeeLeaveBalance> Balances { get; private set; } = new List<EmployeeLeaveBalance>();

    private LeaveType() { }
    public LeaveType(string name, decimal daysPerYear, bool isPaid, string? nameAr = null, string? description = null)
    { Name = name.Trim(); DaysPerYear = daysPerYear; IsPaid = isPaid; NameAr = nameAr?.Trim(); Description = description?.Trim(); }

    public void Update(string name, decimal daysPerYear, bool isPaid, string? nameAr, string? description)
    {
        Name = name.Trim();
        DaysPerYear = daysPerYear;
        IsPaid = isPaid;
        NameAr = nameAr?.Trim();
        Description = description?.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
