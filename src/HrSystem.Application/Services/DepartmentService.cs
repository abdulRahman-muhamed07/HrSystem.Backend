using HrSystem.Application.Exceptions;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Services;

public sealed class DepartmentService(IRepository<Department> departments, IUnitOfWork unitOfWork, IAuditService audit) : IDepartmentService
{
    public async Task<IReadOnlyCollection<DepartmentDto>> GetAllAsync(CancellationToken ct) => await departments.QueryAsync(d => new DepartmentDto(d.Id, d.Name, d.Description, d.Employees.Count), null, 0, int.MaxValue, ct);

    public async Task<int> CreateAsync(CreateDepartmentRequest request, CancellationToken ct)
    {
        if (await departments.CountAsync(d => d.Name.ToLower() == request.Name.Trim().ToLower(), ct) > 0)
            throw new BusinessRuleException("Department name already exists.");
        var department = new Department(request.Name, request.Description);
        await departments.AddAsync(department, ct); await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Create", nameof(Department), department.Id.ToString(), $"Created department {department.Name}", ct);
        return department.Id;
    }

    public async Task UpdateAsync(int id, CreateDepartmentRequest request, CancellationToken ct)
    {
        var department = await departments.GetByIdAsync(id, ct) ?? throw new NotFoundException("Department was not found.");
        department.Update(request.Name, request.Description); await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Update", nameof(Department), id.ToString(), $"Updated department {department.Name}", ct);
    }
}
