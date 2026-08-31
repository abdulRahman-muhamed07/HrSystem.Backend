namespace HrSystem.Application;

public interface IUserService
{
    Task<IReadOnlyCollection<UserDto>> GetAllAsync(CancellationToken ct);
    Task<UserDto?> GetByIdAsync(int id, CancellationToken ct);
    Task SetActiveAsync(int id, bool isActive, CancellationToken ct);
}
