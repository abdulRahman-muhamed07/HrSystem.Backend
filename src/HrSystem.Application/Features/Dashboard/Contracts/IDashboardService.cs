using HrSystem.Application.Models.Dashboard;

namespace HrSystem.Application.Features.Dashboard.Contracts;

public interface IDashboardService
{
    Task<DashboardDto> GetAsync(int year, int month, CancellationToken ct);
}
