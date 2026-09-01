using HrSystem.Application.Models.Departments;

namespace HrSystem.Application.Features.Departments.Contracts;

public interface IDepartmentService
{
    Task<IReadOnlyCollection<DepartmentDto>> GetAllAsync(CancellationToken ct);
    Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<int> CreateAsync(CreateDepartmentRequest request, CancellationToken ct);
    Task UpdateAsync(int id, CreateDepartmentRequest request, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}
