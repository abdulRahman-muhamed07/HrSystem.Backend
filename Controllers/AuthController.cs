using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Data;
using HrSystem.Backend.Models;
using HrSystem.Backend.Models.Dtos;
using HrSystem.Backend.Services;
using System.Security.Claims;

namespace HrSystem.Backend.Controllers;

[Route("api/Areas/Employee/Auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;
    private readonly IAuditService _audit;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AppDbContext db, IJwtService jwtService, IPasswordService passwordService,
        IAuditService audit, ILogger<AuthController> logger)
    {
        _db = db;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _audit = audit;
        _logger = logger;
    }

    /// <summary>
    /// POST api/Areas/Employee/Auth/login
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "Email and password are required." });

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !_passwordService.Verify(dto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Failed login attempt for {Email}", dto.Email);
            return Unauthorized(new { message = "Invalid email or password." });
        }

        if (!user.IsActive)
            return Unauthorized(new { message = "Your account has been deactivated. Contact your administrator." });

        var token = _jwtService.GenerateToken(
            user.Id, user.Email, user.FullName, user.Role, user.EmployeeId);

        await _audit.LogAsync("Login", "User", user.Id.ToString(), $"User {user.Email} logged in");

        return Ok(new LoginResponseDto
        {
            Token = token,
            Role = user.Role,
            FullName = user.FullName,
            Email = user.Email,
            UserId = user.Id,
            EmployeeId = user.EmployeeId
        });
    }

    /// <summary>
    /// GET api/Areas/Employee/Auth/me
    /// Returns the currently authenticated user's profile.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await _db.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.Role,
            user.EmployeeId,
            EmployeeName = user.Employee?.FullName,
            user.CreatedAt
        });
    }

    /// <summary>
    /// PUT api/Areas/Employee/Auth/change-password
    /// </summary>
    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
            return BadRequest(new { message = "Current and new passwords are required." });

        if (dto.NewPassword.Length < 6)
            return BadRequest(new { message = "New password must be at least 6 characters." });

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found" });

        if (!_passwordService.Verify(dto.CurrentPassword, user.PasswordHash))
            return BadRequest(new { message = "Current password is incorrect." });

        user.PasswordHash = _passwordService.Hash(dto.NewPassword);
        await _db.SaveChangesAsync();

        await _audit.LogAsync("ChangePassword", "User", user.Id.ToString(), $"User {user.Email} changed their password");

        return Ok(new { message = "Password changed successfully." });
    }

    /// <summary>
    /// GET api/WeatherForecast — Health check endpoint used by the frontend's testApiConnection().
    /// </summary>
    [Route("api/WeatherForecast")]
    [HttpGet]
    public IActionResult HealthCheck()
    {
        return Ok(new[]
        {
            new { date = DateTime.Today.ToString("yyyy-MM-dd"), temperatureC = 25, summary = "HR System Backend is running" }
        });
    }
}
