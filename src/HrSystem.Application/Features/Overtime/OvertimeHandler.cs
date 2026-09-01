using HrSystem.Application;
using HrSystem.Application.Models.Overtime;

namespace HrSystem.Application.Features.Overtime;

public sealed class OvertimeHandler(IOvertimeService service)
{
    public Task<IReadOnlyCollection<OvertimeDto>> GetPendingAsync(CancellationToken ct) => service.GetPendingAsync(ct);
    public Task<int> CreateAsync(CreateOvertimeRequest request, CancellationToken ct) => service.CreateAsync(request, ct);
    public Task DecideAsync(int id, bool approve, CancellationToken ct) => service.DecideAsync(id, approve, ct);
}
