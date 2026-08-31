using HrSystem.Domain.Enums;

namespace HrSystem.Domain.Entities;

public sealed class User
{
    public int Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; } = UserRole.Employee;
    public bool IsActive { get; private set; } = true;
    public int? EmployeeId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Employee? Employee { get; private set; }
    private User() { }
    public User(string email, string passwordHash, string fullName, UserRole role, int? employeeId = null)
    { Email = email.Trim().ToLowerInvariant(); PasswordHash = passwordHash; FullName = fullName.Trim(); Role = role; EmployeeId = employeeId; }
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
