using HrSystem.Application.Models.Common;
using HrSystem.Application.Models.Leaves;
using HrSystem.Domain.Enums;

namespace HrSystem.Application.Features.Leaves.Contracts;

public interface ILeaveService
{
    Task<PagedResult<LeaveRequestDto>> GetPagedAsync(int page, int pageSize, LeaveRequestStatus? status, CancellationToken ct);
    Task<int> CreateAsync(CreateLeaveRequest request, CancellationToken ct);
    Task DecideAsync(int id, LeaveDecisionRequest request, CancellationToken ct);
}
