namespace HrSystem.Application;

public interface IDashboardService
{
    Task<DashboardDto> GetAsync(int year, int month, CancellationToken ct);
}
