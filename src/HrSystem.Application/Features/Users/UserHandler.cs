using HrSystem.Application;
using HrSystem.Application.Models.Users;

namespace HrSystem.Application.Features.Users;

public sealed class UserHandler(IUserService service)
{
    public Task<IReadOnlyCollection<UserDto>> GetAllAsync(CancellationToken ct) => service.GetAllAsync(ct);
    public Task<UserDto?> GetByIdAsync(int id, CancellationToken ct) => service.GetByIdAsync(id, ct);
    public Task SetActiveAsync(int id, bool active, CancellationToken ct) => service.SetActiveAsync(id, active, ct);
}
