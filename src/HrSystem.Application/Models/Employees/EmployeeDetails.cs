using HrSystem.Domain.Enums;

namespace HrSystem.Application.Models.Employees;

public sealed record EmployeeDetails(int Id, Guid Version, string FullName, string Email, string? NationalId, string? Phone, string JobTitle, int DepartmentId, string DepartmentName, EmploymentType EmploymentType, EmploymentStatus EmploymentStatus, decimal Salary, decimal HousingAllowance, decimal TransportationAllowance, decimal MealAllowance, DateTime HireDate);
