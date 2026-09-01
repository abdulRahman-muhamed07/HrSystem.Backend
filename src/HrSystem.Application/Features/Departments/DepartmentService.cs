using AutoMapper;
using HrSystem.Application.Exceptions;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Services;

public sealed class DepartmentService(
    IDepartmentRepository departments,
    IRepository<Employee> employees,
    IUnitOfWork unitOfWork,
    IAuditService audit,
    IMapper mapper) : IDepartmentService
{
    public async Task<IReadOnlyCollection<DepartmentDto>> GetAllAsync(CancellationToken ct)
    {
        var entities = await departments.GetAllWithEmployeesAsync(ct);
        return mapper.Map<List<DepartmentDto>>(entities);
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var department = await departments.GetByIdAsync(id, ct);
        if (department is null) return null;

        var employeeCount = await employees.CountAsync(e => e.DepartmentId == id, ct);
        var result = mapper.Map<DepartmentDto>(department);
        return result with { EmployeeCount = employeeCount };
    }

    public async Task<int> CreateAsync(CreateDepartmentRequest request, CancellationToken ct)
    {
        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleException("Department name is required.");

        if (await departments.CountAsync(d => d.Name.ToLower() == name.ToLower(), ct) > 0)
            throw new BusinessRuleException("Department name already exists.");

        var department = new Department(name, request.Description);
        await departments.AddAsync(department, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Create", nameof(Department), department.Id.ToString(), $"Created department {department.Name}", ct);
        return department.Id;
    }

    public async Task UpdateAsync(int id, CreateDepartmentRequest request, CancellationToken ct)
    {
        var department = await departments.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Department was not found.");

        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleException("Department name is required.");

        if (await departments.CountAsync(d => d.Name.ToLower() == name.ToLower() && d.Id != id, ct) > 0)
            throw new BusinessRuleException("Department name already exists.");

        department.Update(name, request.Description);
        await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Update", nameof(Department), id.ToString(), $"Updated department {department.Name}", ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var department = await departments.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Department was not found.");

        if (await employees.CountAsync(e => e.DepartmentId == id, ct) > 0)
            throw new BusinessRuleException("A department with employees cannot be deleted.");

        departments.Remove(department);
        await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Delete", nameof(Department), id.ToString(), $"Deleted department {department.Name}", ct);
    }
}
