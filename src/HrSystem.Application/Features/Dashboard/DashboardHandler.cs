using HrSystem.Application;
using HrSystem.Application.Models.Dashboard;

namespace HrSystem.Application.Features.Dashboard;

public sealed class DashboardHandler(IDashboardService service)
{
    public Task<DashboardDto> GetAsync(int year, int month, CancellationToken ct) => service.GetAsync(year, month, ct);
}
