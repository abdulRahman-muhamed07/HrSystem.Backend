using HrSystem.Application.Models.Leaves;

namespace HrSystem.Application.Features.Leaves.Contracts;

public interface ILeaveTypeService
{
    Task<IReadOnlyCollection<LeaveTypeDto>> GetAllAsync(CancellationToken ct);
    Task<LeaveTypeDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<int> CreateAsync(CreateLeaveTypeRequest request, CancellationToken ct);
    Task UpdateAsync(int id, CreateLeaveTypeRequest request, CancellationToken ct);
    Task SetActiveAsync(int id, bool active, CancellationToken ct);
}
