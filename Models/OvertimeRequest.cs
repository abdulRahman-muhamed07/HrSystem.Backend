namespace HrSystem.Backend.Models;

public class OvertimeRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public decimal RateMultiplier { get; set; } = 1.25m;  // 1.25x normal day, 1.5x Friday/night, 2x holiday
    public string? Reason { get; set; }
    public string Status { get; set; } = "Pending";        // Pending, Approved, Rejected
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Employee? Employee { get; set; }
}
