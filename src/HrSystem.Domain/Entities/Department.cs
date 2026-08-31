namespace HrSystem.Domain.Entities;

public sealed class Department
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ICollection<Employee> Employees { get; private set; } = new List<Employee>();

    private Department() { }
    public Department(string name, string? description = null) { Name = name.Trim(); Description = description?.Trim(); }
    public void Update(string name, string? description) { Name = name.Trim(); Description = description?.Trim(); }
}
