namespace HrSystem.Application;

public interface IUserService
{
    Task<IReadOnlyCollection<UserDto>> GetAllAsync(CancellationToken ct);
}
