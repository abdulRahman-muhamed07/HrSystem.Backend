using HrSystem.Domain.Enums;

namespace HrSystem.Domain.Entities;

public sealed class Employee : IConcurrencyTracked
{
    public int Id { get; private set; }
    public Guid Version { get; private set; } = Guid.NewGuid();
    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? NationalId { get; private set; }
    public string? Phone { get; private set; }
    public string? Gender { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public string? MaritalStatus { get; private set; }
    public string? Address { get; private set; }
    public string? ProfilePhotoPath { get; private set; }
    public EmploymentType EmploymentType { get; private set; } = EmploymentType.FullTime;
    public EmploymentStatus EmploymentStatus { get; private set; } = EmploymentStatus.Active;
    public DateTime? ResignationDate { get; private set; }
    public string JobTitle { get; private set; } = string.Empty;
    public int DepartmentId { get; private set; }
    public decimal Salary { get; private set; }
    public decimal HousingAllowance { get; private set; }
    public decimal TransportationAllowance { get; private set; }
    public decimal MealAllowance { get; private set; }
    public DateTime HireDate { get; private set; } = DateTime.UtcNow;
    public DateTime? ContractStartDate { get; private set; }
    public DateTime? ContractEndDate { get; private set; }
    public string? BankName { get; private set; }
    public string? BankAccountNumber { get; private set; }
    public Department? Department { get; private set; }
    public ICollection<AttendanceRecord> AttendanceRecords { get; private set; } = new List<AttendanceRecord>();
    public ICollection<LeaveRequest> LeaveRequests { get; private set; } = new List<LeaveRequest>();
    public ICollection<EmployeeLeaveBalance> LeaveBalances { get; private set; } = new List<EmployeeLeaveBalance>();
    public ICollection<OvertimeRequest> OvertimeRequests { get; private set; } = new List<OvertimeRequest>();
    public ICollection<EmployeeLoan> Loans { get; private set; } = new List<EmployeeLoan>();
    public ICollection<PayrollRecord> PayrollRecords { get; private set; } = new List<PayrollRecord>();
    private Employee() { }
    public Employee(string fullName, string email, string jobTitle, int departmentId, decimal salary, DateTime hireDate)
    { FullName = fullName.Trim(); Email = email.Trim().ToLowerInvariant(); JobTitle = jobTitle.Trim(); DepartmentId = departmentId; Salary = salary; HireDate = hireDate; }
    public void UpdateProfile(string fullName, string email, string jobTitle, int departmentId, decimal salary, EmploymentType employmentType, EmploymentStatus employmentStatus, string? phone, string? address)
    { FullName = fullName.Trim(); Email = email.Trim().ToLowerInvariant(); JobTitle = jobTitle.Trim(); DepartmentId = departmentId; Salary = salary; EmploymentType = employmentType; EmploymentStatus = employmentStatus; Phone = phone?.Trim(); Address = address?.Trim(); if (employmentStatus == EmploymentStatus.Terminated || employmentStatus == EmploymentStatus.Resigned) ResignationDate = DateTime.UtcNow; }
    public void UpdateAllowances(decimal housing, decimal transportation, decimal meal) { HousingAllowance = housing; TransportationAllowance = transportation; MealAllowance = meal; }
}
