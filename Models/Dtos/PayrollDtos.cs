namespace HrSystem.Backend.Models.Dtos;

public class PayrollDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportationAllowance { get; set; }
    public decimal MealAllowance { get; set; }
    public decimal OtherAllowances { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal GosiEmployee { get; set; }
    public decimal GosiEmployer { get; set; }
    public decimal IncomeTax { get; set; }
    public decimal LoanDeduction { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal NetSalary { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PaidAt { get; set; }
}

public class PayrollRunRequestDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int? EmployeeId { get; set; }     // null = run for all employees
    public decimal? OtherAllowances { get; set; }
    public decimal? OtherDeductions { get; set; }
}

public class PayrollRunResultDto
{
    public int Processed { get; set; }
    public decimal TotalGross { get; set; }
    public decimal TotalNet { get; set; }
    public List<PayrollDto> Records { get; set; } = new();
}
