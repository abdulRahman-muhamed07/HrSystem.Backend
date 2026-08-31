using HrSystem.Domain.Enums;

namespace HrSystem.Application;

public sealed record UpdateEmployeeRequest(string FullName, string Email, string JobTitle, int DepartmentId, decimal Salary, EmploymentType EmploymentType, EmploymentStatus EmploymentStatus, string? Phone, string? Address, decimal HousingAllowance, decimal TransportationAllowance, decimal MealAllowance);
