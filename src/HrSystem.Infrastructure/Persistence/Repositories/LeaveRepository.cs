using HrSystem.Application;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Persistence.Repositories;

public sealed class LeaveRepository(AppDbContext db) : EfRepository<LeaveRequest>(db), ILeaveRepository
{
    public Task<bool> HasOverlapAsync(int employeeId, DateTime start, DateTime end, int? excludingId = null, CancellationToken cancellationToken = default) =>
        Query().AsNoTracking().AnyAsync(
            l => l.EmployeeId == employeeId
                 && (!excludingId.HasValue || l.Id != excludingId.Value)
                 && l.StartDate <= end
                 && l.EndDate >= start
                 && l.Status != LeaveRequestStatus.Rejected,
            cancellationToken);
}
