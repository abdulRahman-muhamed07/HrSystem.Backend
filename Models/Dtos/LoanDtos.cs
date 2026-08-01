namespace HrSystem.Backend.Models.Dtos;

public class LoanDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Installments { get; set; }
    public decimal MonthlyDeduction { get; set; }
    public decimal RemainingAmount { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = string.Empty;
    public string RequestDate { get; set; } = string.Empty;
}

public class LoanCreateDto
{
    public decimal Amount { get; set; }
    public int Installments { get; set; }
    public string? Reason { get; set; }
}

public class LoanStatusDto
{
    public string Status { get; set; } = string.Empty; // Approved, Rejected
}
