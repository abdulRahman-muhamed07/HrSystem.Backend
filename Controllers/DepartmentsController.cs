using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Data;
using HrSystem.Backend.Models;
using HrSystem.Backend.Models.Dtos;
using HrSystem.Backend.Services;

namespace HrSystem.Backend.Controllers;

[Route("api/Areas/Admin/Department")]
[ApiController]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;

    public DepartmentsController(AppDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    /// <summary>
    /// GET api/Areas/Admin/Department
    /// Returns all departments with employee counts.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var departments = await _db.Departments
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                EmployeeCount = d.Employees.Count
            })
            .ToListAsync();

        return Ok(departments);
    }

    /// <summary>
    /// POST api/Areas/Admin/Department
    /// Create a new department.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create([FromBody] DepartmentCreateDto dto)
    {
        var exists = await _db.Departments.AnyAsync(d => d.Name == dto.Name);
        if (exists)
            return BadRequest(new { message = "A department with this name already exists." });

        var department = new Department
        {
            Name = dto.Name,
            Description = dto.Description
        };

        _db.Departments.Add(department);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("Create", "Department", department.Id.ToString(), $"Created department {department.Name}");

        return CreatedAtAction(nameof(GetAll), new { id = department.Id }, new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            EmployeeCount = 0
        });
    }

    /// <summary>
    /// PUT api/Areas/Admin/Department/{id}
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DepartmentCreateDto dto)
    {
        var dept = await _db.Departments.FindAsync(id);
        if (dept == null)
            return NotFound(new { message = "Department not found" });

        dept.Name = dto.Name;
        dept.Description = dto.Description;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("Update", "Department", dept.Id.ToString(), $"Updated department {dept.Name}");

        return NoContent();
    }

    /// <summary>
    /// DELETE api/Areas/Admin/Department/{id}
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var dept = await _db.Departments
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dept == null)
            return NotFound(new { message = "Department not found" });

        if (dept.Employees.Any())
            return BadRequest(new { message = "Cannot delete department with assigned employees." });

        _db.Departments.Remove(dept);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("Delete", "Department", id.ToString(), $"Deleted department {dept.Name}");

        return NoContent();
    }
}