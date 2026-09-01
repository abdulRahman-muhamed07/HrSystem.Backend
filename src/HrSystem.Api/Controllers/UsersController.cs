using HrSystem.Application.Features.Users;
using HrSystem.Application.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin")]
[Route("api/users")]
public sealed class UsersController(UserHandler handler) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<UserDto>> GetAll(CancellationToken ct) => handler.GetAllAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken ct)
        => (await handler.GetByIdAsync(id, ct)) is { } user ? Ok(user) : NotFound();

    [HttpPatch("{id:int}/active")]
    public async Task<IActionResult> SetActive(int id, [FromQuery] bool isActive, CancellationToken ct)
    {
        await handler.SetActiveAsync(id, isActive, ct);
        return NoContent();
    }
}
