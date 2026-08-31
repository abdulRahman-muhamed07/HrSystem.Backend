namespace HrSystem.Application;

public interface ILeaveTypeService
{
    Task<IReadOnlyCollection<LeaveTypeDto>> GetAllAsync(CancellationToken ct);
}
