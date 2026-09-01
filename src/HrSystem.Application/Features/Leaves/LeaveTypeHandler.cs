using HrSystem.Application;
using HrSystem.Application.Models.Leaves;

namespace HrSystem.Application.Features.Leaves;

public sealed class LeaveTypeHandler(ILeaveTypeService service)
{
    public Task<IReadOnlyCollection<LeaveTypeDto>> GetAllAsync(CancellationToken ct) => service.GetAllAsync(ct);
    public Task<LeaveTypeDto?> GetByIdAsync(int id, CancellationToken ct) => service.GetByIdAsync(id, ct);
    public Task<int> CreateAsync(CreateLeaveTypeRequest request, CancellationToken ct) => service.CreateAsync(request, ct);
    public Task UpdateAsync(int id, CreateLeaveTypeRequest request, CancellationToken ct) => service.UpdateAsync(id, request, ct);
    public Task SetActiveAsync(int id, bool active, CancellationToken ct) => service.SetActiveAsync(id, active, ct);
}
