using HrSystem.Domain.Enums;

namespace HrSystem.Application;

public interface ILeaveService
{
    Task<int> CreateAsync(CreateLeaveRequest request, CancellationToken ct);
    Task<PagedResult<LeaveRequestDto>> GetPagedAsync(int page, int pageSize, LeaveRequestStatus? status, CancellationToken ct);
    Task DecideAsync(int id, LeaveDecisionRequest request, CancellationToken ct);
}
