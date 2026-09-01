namespace HrSystem.Application.Models.Departments;

public sealed record DepartmentDto(int Id, string Name, string? Description, int EmployeeCount);
