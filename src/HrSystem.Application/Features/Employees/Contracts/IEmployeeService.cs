namespace HrSystem.Application;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeListItem>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct);
    Task<EmployeeDetails?> GetAsync(int id, CancellationToken ct);
    Task<int> CreateAsync(CreateEmployeeRequest request, CancellationToken ct);
    Task UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}
