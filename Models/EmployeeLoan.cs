namespace HrSystem.Backend.Models;

public class EmployeeLoan
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public decimal Amount { get; set; }
    public int Installments { get; set; }
    public decimal MonthlyDeduction { get; set; }
    public decimal RemainingAmount { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "Pending";        // Pending, Approved, Rejected, Completed
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Employee? Employee { get; set; }
}
