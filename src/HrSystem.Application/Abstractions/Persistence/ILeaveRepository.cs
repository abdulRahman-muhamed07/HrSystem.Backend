using HrSystem.Domain.Entities;

namespace HrSystem.Application.Abstractions.Persistence;

public interface ILeaveRepository : IRepository<LeaveRequest>
{
    Task<bool> HasOverlapAsync(int employeeId, DateTime start, DateTime end, int? excludingId = null, CancellationToken cancellationToken = default);
}
