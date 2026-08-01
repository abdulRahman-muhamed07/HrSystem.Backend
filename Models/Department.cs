namespace HrSystem.Backend.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}