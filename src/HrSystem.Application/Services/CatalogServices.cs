using AutoMapper;
using HrSystem.Application.Exceptions;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Services;

public sealed class LeaveTypeService(
    IRepository<LeaveType> types,
    IUnitOfWork unitOfWork,
    IAuditService audit,
    IMapper mapper) : ILeaveTypeService
{
    public async Task<IReadOnlyCollection<LeaveTypeDto>> GetAllAsync(CancellationToken ct)
    {
        var entities = await types.QueryAsync(t => t, null, 0, int.MaxValue, ct);
        return mapper.Map<List<LeaveTypeDto>>(entities);
    }

    public async Task<LeaveTypeDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var entity = await types.GetByIdAsync(id, ct);
        return entity is null ? null : mapper.Map<LeaveTypeDto>(entity);
    }

    public async Task<int> CreateAsync(CreateLeaveTypeRequest request, CancellationToken ct)
    {
        Validate(request);
        var name = request.Name.Trim();
        if (await types.CountAsync(x => x.Name.ToLower() == name.ToLower(), ct) > 0)
            throw new BusinessRuleException("Leave type name already exists.");

        var entity = new LeaveType(name, request.DaysPerYear, request.IsPaid, request.NameAr, request.Description);
        await types.AddAsync(entity, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Create", nameof(LeaveType), entity.Id.ToString(), $"Created leave type {entity.Name}", ct);
        return entity.Id;
    }

    public async Task UpdateAsync(int id, CreateLeaveTypeRequest request, CancellationToken ct)
    {
        Validate(request);
        var entity = await types.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Leave type was not found.");

        var name = request.Name.Trim();
        if (await types.CountAsync(x => x.Id != id && x.Name.ToLower() == name.ToLower(), ct) > 0)
            throw new BusinessRuleException("Leave type name already exists.");

        entity.Update(name, request.DaysPerYear, request.IsPaid, request.NameAr, request.Description);
        await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Update", nameof(LeaveType), id.ToString(), $"Updated leave type {entity.Name}", ct);
    }

    public async Task SetActiveAsync(int id, bool isActive, CancellationToken ct)
    {
        var entity = await types.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Leave type was not found.");

        if (isActive) entity.Activate();
        else entity.Deactivate();

        await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync(isActive ? "Activate" : "Deactivate", nameof(LeaveType), id.ToString(), null, ct);
    }

    private static void Validate(CreateLeaveTypeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BusinessRuleException("Leave type name is required.");
        if (request.DaysPerYear <= 0)
            throw new BusinessRuleException("Days per year must be positive.");
    }
}

public sealed class LeaveBalanceReadService(IRepository<EmployeeLeaveBalance> balances) : ILeaveBalanceReadService
{
    public async Task<IReadOnlyCollection<LeaveBalanceDto>> GetAsync(int employeeId, int year, CancellationToken ct)
        => await balances.QueryAsync(
            b => new LeaveBalanceDto(b.Id, b.EmployeeId, b.LeaveTypeId, b.LeaveType!.Name, b.Year, b.EntitledDays, b.UsedDays, b.AdjustedDays, b.EntitledDays + b.AdjustedDays - b.UsedDays),
            b => b.EmployeeId == employeeId && b.Year == year,
            0,
            int.MaxValue,
            ct);
}

public sealed class AuditLogService(IRepository<AuditLog> logs) : IAuditLogService
{
    public async Task<IReadOnlyCollection<AuditLogDto>> GetRecentAsync(int take, CancellationToken ct)
        => await logs.QueryAsync(
            l => new AuditLogDto(l.Id, l.UserId, l.UserName, l.Action, l.EntityName, l.EntityId, l.Details, l.Timestamp),
            null,
            0,
            Math.Clamp(take, 1, 200),
            ct);
}
