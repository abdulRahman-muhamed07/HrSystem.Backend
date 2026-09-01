using AutoMapper;
using FluentValidation;
using HrSystem.Application.Exceptions;
using HrSystem.Application.Models.Employees;
using HrSystem.Application.Validation;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Enums;

namespace HrSystem.Application.Services;

public sealed class EmployeeService(
    IEmployeeRepository employees,
    IRepository<Department> departments,
    IUnitOfWork unitOfWork,
    IAuditService audit,
    IMapper mapper,
    IValidator<CreateEmployeeRequest> createValidator,
    IValidator<UpdateEmployeeRequest> updateValidator) : IEmployeeService
{
    public async Task<PagedResult<EmployeeListItem>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var term = search?.Trim();
        var total = await employees.CountSearchAsync(term, ct);
        var entities = await employees.SearchAsync(term, page, pageSize, ct);
        return new(mapper.Map<List<EmployeeListItem>>(entities), page, pageSize, total);
    }

    public async Task<EmployeeDetails?> GetAsync(int id, CancellationToken ct)
    {
        var employee = await employees.GetWithDepartmentAsync(id, ct);
        return employee is null ? null : mapper.Map<EmployeeDetails>(employee);
    }

    public async Task<int> CreateAsync(CreateEmployeeRequest request, CancellationToken ct)
    {
        await createValidator.ValidateApplicationRequestAsync(request, ct);
        if (await departments.GetByIdAsync(request.DepartmentId, ct) is null)
            throw new NotFoundException("Department was not found.");

        var email = request.Email.Trim().ToLowerInvariant();
        if (await employees.EmailExistsAsync(email, cancellationToken: ct))
            throw new BusinessRuleException("Email is already used by another employee.");

        var employee = new Employee(request.FullName, email, request.JobTitle, request.DepartmentId, request.Salary, request.HireDate);
        employee.UpdateProfile(request.FullName, email, request.JobTitle, request.DepartmentId, request.Salary,
            request.EmploymentType, EmploymentStatus.Active, request.Phone, request.Address);

        await employees.AddAsync(employee, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Create", nameof(Employee), employee.Id.ToString(), $"Created employee {employee.FullName}", ct);
        return employee.Id;
    }

    public async Task UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken ct)
    {
        await updateValidator.ValidateApplicationRequestAsync(request, ct);
        var employee = await employees.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Employee was not found.");

        if (employee.Version != request.Version)
            throw new ConcurrencyConflictException();
        if (await departments.GetByIdAsync(request.DepartmentId, ct) is null)
            throw new NotFoundException("Department was not found.");

        var email = request.Email.Trim().ToLowerInvariant();
        if (await employees.EmailExistsAsync(email, id, ct))
            throw new BusinessRuleException("Email is already used by another employee.");

        employee.UpdateProfile(request.FullName, email, request.JobTitle, request.DepartmentId, request.Salary,
            request.EmploymentType, request.EmploymentStatus, request.Phone, request.Address);
        employee.UpdateAllowances(request.HousingAllowance, request.TransportationAllowance, request.MealAllowance);

        await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Update", nameof(Employee), id.ToString(), $"Updated employee {employee.FullName}", ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var employee = await employees.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Employee was not found.");

        employee.UpdateProfile(employee.FullName, employee.Email, employee.JobTitle, employee.DepartmentId, employee.Salary,
            employee.EmploymentType, EmploymentStatus.Terminated, employee.Phone, employee.Address);

        await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Deactivate", nameof(Employee), id.ToString(), $"Employee {employee.FullName} was marked as terminated.", ct);
    }
}
