using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Data;
using HrSystem.Backend.Models;
using HrSystem.Backend.Models.Dtos;
using HrSystem.Backend.Services;

namespace HrSystem.Backend.Controllers;

[Route("api/Areas/Admin/Employee")]
[ApiController]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;
    private readonly ILeaveBalanceService _balanceService;

    public EmployeesController(AppDbContext db, IAuditService audit, ILeaveBalanceService balanceService)
    {
        _db = db;
        _audit = audit;
        _balanceService = balanceService;
    }

    /// <summary>
    /// GET api/Areas/Admin/Employee
    /// Returns all employees with department names.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var employees = await _db.Employees
            .Include(e => e.Department)
            .OrderByDescending(e => e.Id)
            .Select(e => ToDto(e))
            .ToListAsync();

        return Ok(employees);
    }

    /// <summary>
    /// GET api/Areas/Admin/Employee/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id)
    {
        var emp = await _db.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (emp == null) return NotFound(new { message = "Employee not found" });

        return Ok(ToDto(emp));
    }

    /// <summary>
    /// POST api/Areas/Admin/Employee
    /// Create a new employee and initialize leave balances.
    /// </summary>
    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create([FromBody] EmployeeCreateDto dto)
    {
        var exists = await _db.Employees.AnyAsync(e => e.Email == dto.Email);
        if (exists)
            return BadRequest(new { message = "An employee with this email already exists." });

        if (!await _db.Departments.AnyAsync(d => d.Id == dto.DepartmentId))
            return BadRequest(new { message = "Department not found." });

        var employee = MapDto(dto);

        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();

        // Initialize leave balances for current year
        var leaveTypes = await _db.LeaveTypes.Where(t => t.IsActive).ToListAsync();
        foreach (var lt in leaveTypes)
            await _balanceService.EnsureBalanceAsync(_db, employee.Id, lt.Id, DateTime.Today.Year);
        await _db.SaveChangesAsync();

        await _db.Entry(employee).Reference(e => e.Department).LoadAsync();
        await _audit.LogAsync("Create", "Employee", employee.Id.ToString(), $"Created employee {employee.FullName}");

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, ToDto(employee));
    }

    /// <summary>
    /// PUT api/Areas/Admin/Employee/{id}
    /// </summary>
    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] EmployeeCreateDto dto)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null)
            return NotFound(new { message = "Employee not found" });

        var emailExists = await _db.Employees
            .AnyAsync(e => e.Email == dto.Email && e.Id != id);
        if (emailExists)
            return BadRequest(new { message = "An employee with this email already exists." });

        var old = $"{employee.FullName}";
        employee.FullName = dto.FullName;
        employee.Email = dto.Email;
        employee.NationalId = dto.NationalId;
        employee.Phone = dto.Phone;
        employee.Gender = dto.Gender;
        employee.BirthDate = ParseDate(dto.BirthDate);
        employee.MaritalStatus = dto.MaritalStatus;
        employee.Address = dto.Address;
        employee.EmploymentType = dto.EmploymentType;
        employee.EmploymentStatus = dto.EmploymentStatus;
        employee.ResignationDate = ParseDate(dto.ResignationDate);
        employee.DepartmentId = dto.DepartmentId;
        employee.JobTitle = dto.JobTitle;
        employee.Salary = dto.Salary;
        employee.HousingAllowance = dto.HousingAllowance;
        employee.TransportationAllowance = dto.TransportationAllowance;
        employee.MealAllowance = dto.MealAllowance;
        employee.HireDate = ParseDate(dto.HireDate) ?? employee.HireDate;
        employee.ContractStartDate = ParseDate(dto.ContractStartDate);
        employee.ContractEndDate = ParseDate(dto.ContractEndDate);
        employee.BankName = dto.BankName;
        employee.BankAccountNumber = dto.BankAccountNumber;

        await _db.SaveChangesAsync();

        await _audit.LogAsync("Update", "Employee", employee.Id.ToString(), $"Updated employee {employee.FullName}");

        return NoContent();
    }

    /// <summary>
    /// DELETE api/Areas/Admin/Employee/{id}
    /// </summary>
    [Authorize(Roles = "Admin,HR")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null)
            return NotFound(new { message = "Employee not found" });

        var linkedUser = await _db.Users.AnyAsync(u => u.EmployeeId == id);
        if (linkedUser)
            return BadRequest(new { message = "Cannot delete an employee that has a linked user account. Deactivate the account instead." });

        _db.Employees.Remove(employee);
        await _db.SaveChangesAsync();

        await _audit.LogAsync("Delete", "Employee", id.ToString(), $"Deleted employee {employee.FullName}");

        return NoContent();
    }

    private static DateTime? ParseDate(string? s)
        => DateTime.TryParse(s, out var d) ? d : (DateTime?)null;

    private static Employee MapDto(EmployeeCreateDto dto) => new()
    {
        FullName = dto.FullName,
        Email = dto.Email,
        NationalId = dto.NationalId,
        Phone = dto.Phone,
        Gender = dto.Gender,
        BirthDate = ParseDate(dto.BirthDate),
        MaritalStatus = dto.MaritalStatus,
        Address = dto.Address,
        EmploymentType = dto.EmploymentType,
        EmploymentStatus = dto.EmploymentStatus,
        ResignationDate = ParseDate(dto.ResignationDate),
        DepartmentId = dto.DepartmentId,
        JobTitle = dto.JobTitle,
        Salary = dto.Salary,
        HousingAllowance = dto.HousingAllowance,
        TransportationAllowance = dto.TransportationAllowance,
        MealAllowance = dto.MealAllowance,
        HireDate = ParseDate(dto.HireDate) ?? DateTime.UtcNow,
        ContractStartDate = ParseDate(dto.ContractStartDate),
        ContractEndDate = ParseDate(dto.ContractEndDate),
        BankName = dto.BankName,
        BankAccountNumber = dto.BankAccountNumber
    };

    private static EmployeeDto ToDto(Employee e) => new()
    {
        Id = e.Id,
        FullName = e.FullName,
        Email = e.Email,
        NationalId = e.NationalId,
        Phone = e.Phone,
        Gender = e.Gender,
        BirthDate = e.BirthDate?.ToString("yyyy-MM-dd"),
        MaritalStatus = e.MaritalStatus,
        Address = e.Address,
        EmploymentType = e.EmploymentType,
        EmploymentStatus = e.EmploymentStatus,
        ResignationDate = e.ResignationDate?.ToString("yyyy-MM-dd"),
        DepartmentId = e.DepartmentId,
        DepartmentName = e.Department?.Name,
        JobTitle = e.JobTitle,
        Salary = e.Salary,
        HousingAllowance = e.HousingAllowance,
        TransportationAllowance = e.TransportationAllowance,
        MealAllowance = e.MealAllowance,
        HireDate = e.HireDate.ToString("yyyy-MM-dd"),
        ContractStartDate = e.ContractStartDate?.ToString("yyyy-MM-dd"),
        ContractEndDate = e.ContractEndDate?.ToString("yyyy-MM-dd"),
        BankName = e.BankName,
        BankAccountNumber = e.BankAccountNumber
    };
}
