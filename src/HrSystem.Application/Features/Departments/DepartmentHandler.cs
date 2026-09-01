using HrSystem.Application;
using HrSystem.Application.Models.Departments;

namespace HrSystem.Application.Features.Departments;

public sealed class DepartmentHandler(IDepartmentService service)
{
    public Task<IReadOnlyCollection<DepartmentDto>> GetAllAsync(CancellationToken ct) => service.GetAllAsync(ct);
    public Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken ct) => service.GetByIdAsync(id, ct);
    public Task<int> CreateAsync(CreateDepartmentRequest request, CancellationToken ct) => service.CreateAsync(request, ct);
    public Task UpdateAsync(int id, CreateDepartmentRequest request, CancellationToken ct) => service.UpdateAsync(id, request, ct);
    public Task DeleteAsync(int id, CancellationToken ct) => service.DeleteAsync(id, ct);
}
