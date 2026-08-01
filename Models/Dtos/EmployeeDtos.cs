namespace HrSystem.Backend.Models.Dtos;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? Phone { get; set; }
    public string? Gender { get; set; }
    public string? BirthDate { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Address { get; set; }
    public string EmploymentType { get; set; } = "FullTime";
    public string EmploymentStatus { get; set; } = "Active";
    public string? ResignationDate { get; set; }
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportationAllowance { get; set; }
    public decimal MealAllowance { get; set; }
    public string HireDate { get; set; } = string.Empty;
    public string? ContractStartDate { get; set; }
    public string? ContractEndDate { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
}

public class EmployeeCreateDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? Phone { get; set; }
    public string? Gender { get; set; }
    public string? BirthDate { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Address { get; set; }
    public string EmploymentType { get; set; } = "FullTime";
    public string EmploymentStatus { get; set; } = "Active";
    public string? ResignationDate { get; set; }
    public int DepartmentId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportationAllowance { get; set; }
    public decimal MealAllowance { get; set; }
    public string HireDate { get; set; } = string.Empty;
    public string? ContractStartDate { get; set; }
    public string? ContractEndDate { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
}
