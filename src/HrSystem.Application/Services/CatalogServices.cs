using HrSystem.Domain.Entities;

namespace HrSystem.Application.Services;

public sealed record LeaveTypeDto(int Id, string Name, string? NameAr, decimal DaysPerYear, bool IsPaid, bool IsActive, string? Description);
public sealed record LeaveBalanceDto(int Id, int EmployeeId, int LeaveTypeId, string LeaveTypeName, int Year, decimal EntitledDays, decimal UsedDays, decimal AdjustedDays, decimal AvailableDays);
public sealed record AuditLogDto(int Id, int? UserId, string? UserName, string Action, string EntityName, string? EntityId, string? Details, DateTime Timestamp);

public interface ILeaveTypeService { Task<IReadOnlyCollection<LeaveTypeDto>> GetAllAsync(CancellationToken ct); }
public interface ILeaveBalanceReadService { Task<IReadOnlyCollection<LeaveBalanceDto>> GetAsync(int employeeId, int year, CancellationToken ct); }
public interface IAuditLogService { Task<IReadOnlyCollection<AuditLogDto>> GetRecentAsync(int take, CancellationToken ct); }

public sealed class LeaveTypeService(IRepository<LeaveType> types) : ILeaveTypeService
{
    public async Task<IReadOnlyCollection<LeaveTypeDto>> GetAllAsync(CancellationToken ct) => await types.QueryAsync(t => new LeaveTypeDto(t.Id, t.Name, t.NameAr, t.DaysPerYear, t.IsPaid, t.IsActive, t.Description), null, 0, int.MaxValue, ct);
}

public sealed class LeaveBalanceReadService(IRepository<EmployeeLeaveBalance> balances) : ILeaveBalanceReadService
{
    public async Task<IReadOnlyCollection<LeaveBalanceDto>> GetAsync(int employeeId, int year, CancellationToken ct) => await balances.QueryAsync(b => new LeaveBalanceDto(b.Id, b.EmployeeId, b.LeaveTypeId, b.LeaveType!.Name, b.Year, b.EntitledDays, b.UsedDays, b.AdjustedDays, b.EntitledDays + b.AdjustedDays - b.UsedDays), b => b.EmployeeId == employeeId && b.Year == year, 0, int.MaxValue, ct);
}

public sealed class AuditLogService(IRepository<AuditLog> logs) : IAuditLogService
{
    public async Task<IReadOnlyCollection<AuditLogDto>> GetRecentAsync(int take, CancellationToken ct) => await logs.QueryAsync(l => new AuditLogDto(l.Id, l.UserId, l.UserName, l.Action, l.EntityName, l.EntityId, l.Details, l.Timestamp), null, 0, Math.Clamp(take, 1, 200), ct);
}
