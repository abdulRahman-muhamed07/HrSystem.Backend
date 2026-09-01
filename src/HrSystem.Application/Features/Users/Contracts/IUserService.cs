using HrSystem.Application.Models.Users;

namespace HrSystem.Application.Features.Users.Contracts;

public interface IUserService
{
    Task<IReadOnlyCollection<UserDto>> GetAllAsync(CancellationToken ct);
    Task<UserDto?> GetByIdAsync(int id, CancellationToken ct);
    Task SetActiveAsync(int id, bool active, CancellationToken ct);
}
