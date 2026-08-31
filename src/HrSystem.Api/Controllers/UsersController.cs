using HrSystem.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Controllers;

[ApiController, Authorize(Roles = "Admin")]
[Route("api/users")]
public sealed class UsersController(IUserService service) : ControllerBase
{
    [HttpGet] public Task<IReadOnlyCollection<UserDto>> GetAll(CancellationToken ct) => service.GetAllAsync(ct);
}
