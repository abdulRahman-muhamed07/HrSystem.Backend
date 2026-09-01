using HrSystem.Application;
using HrSystem.Application.Models.Leaves;

namespace HrSystem.Application.Features.LeaveBalances;

public sealed class LeaveBalanceHandler(ILeaveBalanceReadService service)
{
    public Task<IReadOnlyCollection<LeaveBalanceDto>> GetAsync(int employeeId, int year, CancellationToken ct) => service.GetAsync(employeeId, year, ct);
}
