using HrSystem.Domain.Entities;

namespace HrSystem.Application;

public interface ILeaveRepository : IRepository<LeaveRequest>
{
    Task<bool> HasOverlapAsync(int employeeId, DateTime start, DateTime end, int? excludingId = null, CancellationToken cancellationToken = default);
}
