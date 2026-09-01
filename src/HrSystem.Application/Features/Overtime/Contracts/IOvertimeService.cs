using HrSystem.Application.Models.Overtime;

namespace HrSystem.Application.Features.Overtime.Contracts;

public interface IOvertimeService
{
    Task<IReadOnlyCollection<OvertimeDto>> GetPendingAsync(CancellationToken ct);
    Task<int> CreateAsync(CreateOvertimeRequest request, CancellationToken ct);
    Task DecideAsync(int id, bool approve, CancellationToken ct);
}
