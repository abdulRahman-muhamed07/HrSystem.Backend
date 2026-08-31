using AutoMapper;
using HrSystem.Application.Exceptions;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Services;

public sealed class UserService(
    IRepository<User> users,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IUserService
{
    public async Task<IReadOnlyCollection<UserDto>> GetAllAsync(CancellationToken ct)
    {
        var entities = await users.QueryAsync(u => u, null, 0, int.MaxValue, ct);
        return mapper.Map<List<UserDto>>(entities);
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await users.GetByIdAsync(id, ct);
        return entity is null ? null : mapper.Map<UserDto>(entity);
    }

    public async Task SetActiveAsync(int id, bool isActive, CancellationToken ct)
    {
        var entity = await users.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("User was not found.");

        if (isActive) entity.Activate();
        else entity.Deactivate();

        await unitOfWork.SaveChangesAsync(ct);
    }
}
