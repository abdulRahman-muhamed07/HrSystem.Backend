using HrSystem.Application.Exceptions;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Services;

public sealed class UserService(IRepository<User> users) : IUserService
{
    public async Task<IReadOnlyCollection<UserDto>> GetAllAsync(CancellationToken ct) => await users.QueryAsync(u => new UserDto(u.Id, u.Email, u.FullName, u.Role, u.IsActive, u.EmployeeId), null, 0, int.MaxValue, ct);
}
