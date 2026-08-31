using HrSystem.Application.Exceptions;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Enums;

namespace HrSystem.Application.Services;

public sealed class EmployeeService(IRepository<Employee> employees, IRepository<Department> departments, IUnitOfWork unitOfWork, IAuditService audit) : IEmployeeService
{
    public async Task<PagedResult<EmployeeListItem>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct)
    {
        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 1, 100);
        var text = search?.Trim().ToLowerInvariant();
        System.Linq.Expressions.Expression<Func<Employee, bool>>? predicate = string.IsNullOrWhiteSpace(text) ? null : e => e.FullName.ToLower().Contains(text) || e.Email.ToLower().Contains(text) || e.JobTitle.ToLower().Contains(text);
        var total = await employees.CountAsync(predicate, ct);
        var items = await employees.QueryAsync(e => new EmployeeListItem(e.Id, e.FullName, e.Email, e.JobTitle, e.Department!.Name, e.EmploymentStatus, e.Salary), predicate, (page - 1) * pageSize, pageSize, ct);
        return new(items, page, pageSize, total);
    }

    public async Task<EmployeeDetails?> GetAsync(int id, CancellationToken ct)
    {
        var result = await employees.QueryAsync(e => new EmployeeDetails(e.Id, e.FullName, e.Email, e.NationalId, e.Phone, e.JobTitle, e.DepartmentId, e.Department!.Name, e.EmploymentType, e.EmploymentStatus, e.Salary, e.HousingAllowance, e.TransportationAllowance, e.MealAllowance, e.HireDate), e => e.Id == id, 0, 1, ct);
        return result.FirstOrDefault();
    }

    public async Task<int> CreateAsync(CreateEmployeeRequest request, CancellationToken ct)
    {
        if (request.Salary < 0) throw new BusinessRuleException("Salary cannot be negative.");
        if (await departments.GetByIdAsync(request.DepartmentId, ct) is null) throw new NotFoundException("Department was not found.");
        var email = request.Email.Trim().ToLowerInvariant();
        if (await employees.CountAsync(e => e.Email == email, ct) > 0) throw new BusinessRuleException("Email is already used by another employee.");
        var employee = new Employee(request.FullName, email, request.JobTitle, request.DepartmentId, request.Salary, request.HireDate);
        employee.UpdateProfile(request.FullName, email, request.JobTitle, request.DepartmentId, request.Salary, request.EmploymentType, EmploymentStatus.Active, request.Phone, request.Address);
        await employees.AddAsync(employee, ct); await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Create", nameof(Employee), employee.Id.ToString(), $"Created employee {employee.FullName}", ct);
        return employee.Id;
    }

    public async Task UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken ct)
    {
        var employee = await employees.GetByIdAsync(id, ct) ?? throw new NotFoundException("Employee was not found.");
        if (await departments.GetByIdAsync(request.DepartmentId, ct) is null) throw new NotFoundException("Department was not found.");
        var email = request.Email.Trim().ToLowerInvariant();
        if (await employees.CountAsync(e => e.Email == email && e.Id != id, ct) > 0) throw new BusinessRuleException("Email is already used by another employee.");
        employee.UpdateProfile(request.FullName, email, request.JobTitle, request.DepartmentId, request.Salary, request.EmploymentType, request.EmploymentStatus, request.Phone, request.Address);
        employee.UpdateAllowances(request.HousingAllowance, request.TransportationAllowance, request.MealAllowance);
        await unitOfWork.SaveChangesAsync(ct); await audit.WriteAsync("Update", nameof(Employee), id.ToString(), $"Updated employee {employee.FullName}", ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var employee = await employees.GetByIdAsync(id, ct) ?? throw new NotFoundException("Employee was not found.");
        employee.UpdateProfile(employee.FullName, employee.Email, employee.JobTitle, employee.DepartmentId, employee.Salary, employee.EmploymentType, EmploymentStatus.Terminated, employee.Phone, employee.Address);
        await unitOfWork.SaveChangesAsync(ct); await audit.WriteAsync("Deactivate", nameof(Employee), id.ToString(), $"Employee {employee.FullName} was marked as terminated.", ct);
    }
}
