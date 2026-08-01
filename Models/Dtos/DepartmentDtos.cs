namespace HrSystem.Backend.Models.Dtos;

public class DepartmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EmployeeCount { get; set; }
}

public class DepartmentCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}