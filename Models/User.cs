namespace HrSystem.Backend.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "Employee"; // Admin, HR, Employee
    public bool IsActive { get; set; } = true;
    public int? EmployeeId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Employee? Employee { get; set; }
}
