using HrSystem.Application;
using HrSystem.Application.Features.Employees.Contracts;
using HrSystem.Application.Models.Common;
using HrSystem.Application.Models.Employees;

namespace HrSystem.Application.Features.Employees;

public sealed class EmployeeHandler(IEmployeeService service)
{
    public Task<PagedResult<EmployeeListItem>> GetAsync(int page, int pageSize, string? search, CancellationToken ct) => service.GetPagedAsync(page, pageSize, search, ct);
    public Task<EmployeeDetails?> GetByIdAsync(int id, CancellationToken ct) => service.GetAsync(id, ct);
    public Task<int> CreateAsync(CreateEmployeeRequest request, CancellationToken ct) => service.CreateAsync(request, ct);
    public Task UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken ct) => service.UpdateAsync(id, request, ct);
    public Task DeleteAsync(int id, CancellationToken ct) => service.DeleteAsync(id, ct);
}
