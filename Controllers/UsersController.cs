using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Data;
using HrSystem.Backend.Models;
using HrSystem.Backend.Models.Dtos;
using HrSystem.Backend.Services;

namespace HrSystem.Backend.Controllers;

[Route("api/Areas/Admin/Users")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordService _passwordService;
    private readonly IAuditService _audit;

    public UsersController(AppDbContext db, IPasswordService passwordService, IAuditService audit)
    {
        _db = db;
        _passwordService = passwordService;
        _audit = audit;
    }

    /// <summary>
    /// GET api/Areas/Admin/Users
    /// List all user accounts.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var users = await _db.Users
            .Include(u => u.Employee)
            .OrderBy(u => u.FullName)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role,
                EmployeeId = u.EmployeeId,
                EmployeeName = u.Employee != null ? u.Employee.FullName : null,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();
        return Ok(users);
    }

    /// <summary>
    /// POST api/Areas/Admin/Users
    /// Create a user account (optionally linked to an employee).
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] UserCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "Email and password are required." });
        if (dto.Password.Length < 6)
            return BadRequest(new { message = "Password must be at least 6 characters." });
        if (dto.Role != "Admin" && dto.Role != "HR" && dto.Role != "Employee")
            return BadRequest(new { message = "Role must be Admin, HR or Employee." });

        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest(new { message = "A user with this email already exists." });

        if (dto.EmployeeId.HasValue && !await _db.Employees.AnyAsync(e => e.Id == dto.EmployeeId))
            return BadRequest(new { message = "Linked employee not found." });

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = _passwordService.Hash(dto.Password),
            FullName = dto.FullName,
            Role = dto.Role,
            EmployeeId = dto.EmployeeId,
            IsActive = true
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        await _audit.LogAsync("Create", "User", user.Id.ToString(), $"Created user account {user.Email} with role {user.Role}");

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, ToDto(user));
    }

    /// <summary>
    /// GET api/Areas/Admin/Users/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _db.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return NotFound(new { message = "User not found" });
        return Ok(ToDto(user));
    }

    /// <summary>
    /// PUT api/Areas/Admin/Users/{id}
    /// Update user details / role / status.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
                return BadRequest(new { message = "A user with this email already exists." });
            user.Email = dto.Email;
        }
        if (!string.IsNullOrWhiteSpace(dto.FullName)) user.FullName = dto.FullName;
        if (!string.IsNullOrWhiteSpace(dto.Role))
        {
            if (dto.Role != "Admin" && dto.Role != "HR" && dto.Role != "Employee")
                return BadRequest(new { message = "Role must be Admin, HR or Employee." });
            user.Role = dto.Role;
        }
        if (dto.EmployeeId.HasValue)
            user.EmployeeId = dto.EmployeeId == 0 ? null : dto.EmployeeId;
        if (dto.IsActive.HasValue)
            user.IsActive = dto.IsActive.Value;

        await _db.SaveChangesAsync();

        await _audit.LogAsync("Update", "User", user.Id.ToString(), $"Updated user {user.Email}");

        return NoContent();
    }

    /// <summary>
    /// PUT api/Areas/Admin/Users/{id}/reset-password
    /// Admin resets a user's password.
    /// </summary>
    [HttpPut("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] AdminResetPasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
            return BadRequest(new { message = "New password must be at least 6 characters." });

        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        user.PasswordHash = _passwordService.Hash(dto.NewPassword);
        await _db.SaveChangesAsync();

        await _audit.LogAsync("ResetPassword", "User", user.Id.ToString(), $"Admin reset password for {user.Email}");

        return Ok(new { message = "Password reset successfully." });
    }

    /// <summary>
    /// PUT api/Areas/Admin/Users/{id}/activate
    /// Enable or disable a user account.
    /// </summary>
    [HttpPut("{id}/activate")]
    public async Task<IActionResult> SetActive(int id, [FromBody] bool isActive)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        user.IsActive = isActive;
        await _db.SaveChangesAsync();

        await _audit.LogAsync(isActive ? "Activate" : "Deactivate", "User", user.Id.ToString(),
            $"{user.Email} account {(isActive ? "activated" : "deactivated")}");

        return NoContent();
    }

    /// <summary>
    /// DELETE api/Areas/Admin/Users/{id}
    /// Delete a user account.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        await _audit.LogAsync("Delete", "User", id.ToString(), $"Deleted user account {user.Email}");

        return NoContent();
    }

    private static UserDto ToDto(User u) => new()
    {
        Id = u.Id,
        Email = u.Email,
        FullName = u.FullName,
        Role = u.Role,
        EmployeeId = u.EmployeeId,
        EmployeeName = u.Employee?.FullName,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt
    };
}
