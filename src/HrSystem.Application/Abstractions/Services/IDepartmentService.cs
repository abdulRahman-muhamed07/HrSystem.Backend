namespace HrSystem.Application;

public interface IDepartmentService
{
    Task<IReadOnlyCollection<DepartmentDto>> GetAllAsync(CancellationToken ct);
    Task<int> CreateAsync(CreateDepartmentRequest request, CancellationToken ct);
    Task UpdateAsync(int id, CreateDepartmentRequest request, CancellationToken ct);
}
