using HrSystem.Application;
using HrSystem.Application.Models.Leaves;
using HrSystem.Domain.Enums;
using HrSystem.Application.Models.Common;

namespace HrSystem.Application.Features.Leaves;

public sealed class LeaveHandler(ILeaveService service)
{
    public Task<PagedResult<LeaveRequestDto>> GetAsync(int page, int pageSize, LeaveRequestStatus? status, CancellationToken ct) => service.GetPagedAsync(page, pageSize, status, ct);
    public Task<int> CreateAsync(CreateLeaveRequest request, CancellationToken ct) => service.CreateAsync(request, ct);
    public Task DecideAsync(int id, LeaveDecisionRequest request, CancellationToken ct) => service.DecideAsync(id, request, ct);
}
