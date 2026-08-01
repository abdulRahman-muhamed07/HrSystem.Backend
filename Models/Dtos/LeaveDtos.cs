namespace HrSystem.Backend.Models.Dtos;

public class LeaveDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int LeaveTypeId { get; set; }
    public string? LeaveTypeName { get; set; }
    public string? LeaveTypeNameAr { get; set; }
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public decimal DurationDays { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class LeaveCreateDto
{
    public int LeaveTypeId { get; set; }
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class LeaveStatusUpdateDto
{
    public string Status { get; set; } = string.Empty; // Approved, Rejected
    public string? RejectionReason { get; set; }
}

public class LeaveTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public decimal DaysPerYear { get; set; }
    public bool IsPaid { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}

public class LeaveTypeCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public decimal DaysPerYear { get; set; }
    public bool IsPaid { get; set; } = true;
    public string? Description { get; set; }
}

public class LeaveBalanceDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int LeaveTypeId { get; set; }
    public string? LeaveTypeName { get; set; }
    public string? LeaveTypeNameAr { get; set; }
    public int Year { get; set; }
    public decimal EntitledDays { get; set; }
    public decimal UsedDays { get; set; }
    public decimal AdjustedDays { get; set; }
    public decimal RemainingDays { get; set; }
}

public class LeaveBalanceAdjustDto
{
    public decimal AdjustedDays { get; set; }
    public string? Note { get; set; }
}
