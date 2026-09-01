using HrSystem.Application.Models.Leaves;

namespace HrSystem.Application.Features.LeaveBalances.Contracts;

public interface ILeaveBalanceReadService
{
    Task<IReadOnlyCollection<LeaveBalanceDto>> GetAsync(int employeeId, int year, CancellationToken ct);
}
