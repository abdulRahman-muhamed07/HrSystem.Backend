using HrSystem.Domain.Entities;

namespace HrSystem.Application;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<List<Department>> GetAllWithEmployeesAsync(CancellationToken cancellationToken = default);
}
