using HrSystem.Application.Models.Leaves;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Services;

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
