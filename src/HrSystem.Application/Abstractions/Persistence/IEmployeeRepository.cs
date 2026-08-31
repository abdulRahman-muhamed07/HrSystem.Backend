using HrSystem.Domain.Entities;

namespace HrSystem.Application;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<bool> EmailExistsAsync(string email, int? excludingId = null, CancellationToken cancellationToken = default);
    Task<Employee?> GetWithDepartmentAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> SearchAsync(string? term, int page, int pageSize, CancellationToken cancellationToken = default);
}
