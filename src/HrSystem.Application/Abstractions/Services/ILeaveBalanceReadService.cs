using HrSystem.Application.Models.Leaves;

namespace HrSystem.Application;

public interface ILeaveBalanceReadService
{
    Task<IReadOnlyCollection<LeaveBalanceDto>> GetAsync(int employeeId, int year, CancellationToken ct);
}
