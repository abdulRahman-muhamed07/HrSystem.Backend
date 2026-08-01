namespace HrSystem.Backend.Models.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "Employee";
    public int? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class UserCreateDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "Employee";
    public int? EmployeeId { get; set; }
}

public class UserUpdateDto
{
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Role { get; set; }
    public int? EmployeeId { get; set; }
    public bool? IsActive { get; set; }
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class AdminResetPasswordDto
{
    public string NewPassword { get; set; } = string.Empty;
}
