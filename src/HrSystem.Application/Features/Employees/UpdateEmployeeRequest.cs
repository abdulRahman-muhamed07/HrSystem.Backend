using HrSystem.Domain.Enums;

namespace HrSystem.Application.Models.Employees;

public sealed record UpdateEmployeeRequest(Guid Version, string FullName, string Email, string JobTitle, int DepartmentId, decimal Salary, EmploymentType EmploymentType, EmploymentStatus EmploymentStatus, string? Phone, string? Address, decimal HousingAllowance, decimal TransportationAllowance, decimal MealAllowance);
