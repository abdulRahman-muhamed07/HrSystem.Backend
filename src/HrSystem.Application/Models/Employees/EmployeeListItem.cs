using HrSystem.Domain.Enums;

namespace HrSystem.Application.Models.Employees;

public sealed record EmployeeListItem(int Id, string FullName, string Email, string JobTitle, string DepartmentName, EmploymentStatus Status, decimal Salary);
