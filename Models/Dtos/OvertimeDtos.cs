namespace HrSystem.Backend.Models.Dtos;

public class OvertimeDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public decimal RateMultiplier { get; set; }
    public decimal EstimatedPay { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class OvertimeCreateDto
{
    public string Date { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public decimal RateMultiplier { get; set; } = 1.25m;
    public string? Reason { get; set; }
}

public class OvertimeStatusDto
{
    public string Status { get; set; } = string.Empty; // Approved, Rejected
}
