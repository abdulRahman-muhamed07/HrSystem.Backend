using HrSystem.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin")]
[Route("api/users")]
public sealed class UsersController(IUserService service) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<UserDto>> GetAll(CancellationToken ct)
        => service.GetAllAsync(ct);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken ct)
        => (await service.GetByIdAsync(id, ct)) is { } user ? Ok(user) : NotFound();

    [HttpPatch("{id:int}/active")]
    public async Task<IActionResult> SetActive(int id, [FromQuery] bool isActive, CancellationToken ct)
    {
        await service.SetActiveAsync(id, isActive, ct);
        return NoContent();
    }
}
