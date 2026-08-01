using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrSystem.Backend.Data;
using HrSystem.Backend.Models;
using HrSystem.Backend.Models.Dtos;
using HrSystem.Backend.Services;

namespace HrSystem.Backend.Controllers;

[Route("api/Areas/Admin/LeaveTypes")]
[ApiController]
[Authorize]
public class LeaveTypesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;

    public LeaveTypesController(AppDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var types = await _db.LeaveTypes
            .OrderBy(t => t.Id)
            .Select(t => new LeaveTypeDto
            {
                Id = t.Id,
                Name = t.Name,
                NameAr = t.NameAr,
                DaysPerYear = t.DaysPerYear,
                IsPaid = t.IsPaid,
                IsActive = t.IsActive,
                Description = t.Description
            })
            .ToListAsync();
        return Ok(types);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LeaveTypeDto>> GetById(int id)
    {
        var t = await _db.LeaveTypes.FindAsync(id);
        if (t == null) return NotFound(new { message = "Leave type not found" });
        return Ok(new LeaveTypeDto
        {
            Id = t.Id,
            Name = t.Name,
            NameAr = t.NameAr,
            DaysPerYear = t.DaysPerYear,
            IsPaid = t.IsPaid,
            IsActive = t.IsActive,
            Description = t.Description
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<LeaveTypeDto>> Create([FromBody] LeaveTypeCreateDto dto)
    {
        if (await _db.LeaveTypes.AnyAsync(t => t.Name == dto.Name))
            return BadRequest(new { message = "A leave type with this name already exists." });

        var type = new LeaveType
        {
            Name = dto.Name,
            NameAr = dto.NameAr,
            DaysPerYear = dto.DaysPerYear,
            IsPaid = dto.IsPaid,
            IsActive = true,
            Description = dto.Description
        };
        _db.LeaveTypes.Add(type);
        await _db.SaveChangesAsync();

        await _audit.LogAsync("Create", "LeaveType", type.Id.ToString(), $"Created leave type {type.Name}");

        return CreatedAtAction(nameof(GetById), new { id = type.Id }, new LeaveTypeDto
        {
            Id = type.Id, Name = type.Name, NameAr = type.NameAr,
            DaysPerYear = type.DaysPerYear, IsPaid = type.IsPaid,
            IsActive = type.IsActive, Description = type.Description
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] LeaveTypeCreateDto dto)
    {
        var type = await _db.LeaveTypes.FindAsync(id);
        if (type == null) return NotFound(new { message = "Leave type not found" });

        var nameExists = await _db.LeaveTypes.AnyAsync(t => t.Name == dto.Name && t.Id != id);
        if (nameExists)
            return BadRequest(new { message = "A leave type with this name already exists." });

        type.Name = dto.Name;
        type.NameAr = dto.NameAr;
        type.DaysPerYear = dto.DaysPerYear;
        type.IsPaid = dto.IsPaid;
        type.Description = dto.Description;
        await _db.SaveChangesAsync();

        await _audit.LogAsync("Update", "LeaveType", type.Id.ToString(), $"Updated leave type {type.Name}");

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var type = await _db.LeaveTypes.FindAsync(id);
        if (type == null) return NotFound(new { message = "Leave type not found" });

        if (await _db.LeaveRequests.AnyAsync(l => l.LeaveTypeId == id))
            return BadRequest(new { message = "Cannot delete a leave type that has requests. Deactivate it instead." });

        _db.LeaveTypes.Remove(type);
        await _db.SaveChangesAsync();

        await _audit.LogAsync("Delete", "LeaveType", id.ToString(), $"Deleted leave type {type.Name}");

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/activate")]
    public async Task<IActionResult> ToggleActive(int id, [FromBody] bool isActive)
    {
        var type = await _db.LeaveTypes.FindAsync(id);
        if (type == null) return NotFound(new { message = "Leave type not found" });

        type.IsActive = isActive;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
