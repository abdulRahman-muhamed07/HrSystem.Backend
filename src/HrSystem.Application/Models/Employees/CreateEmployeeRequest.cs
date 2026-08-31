using HrSystem.Domain.Enums;

namespace HrSystem.Application;

public sealed record CreateEmployeeRequest(string FullName, string Email, string JobTitle, int DepartmentId, decimal Salary, DateTime HireDate, EmploymentType EmploymentType = EmploymentType.FullTime, string? Phone = null, string? Address = null);
