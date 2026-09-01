namespace HrSystem.Application;

public interface IOvertimeService
{
    Task<int> CreateAsync(CreateOvertimeRequest request, CancellationToken ct);
    Task<IReadOnlyCollection<OvertimeDto>> GetPendingAsync(CancellationToken ct);
    Task DecideAsync(int id, bool approve, CancellationToken ct);
}
