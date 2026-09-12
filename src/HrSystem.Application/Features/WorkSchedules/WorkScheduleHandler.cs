using HrSystem.Application;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Features.WorkSchedules;

public sealed record WorkScheduleDto(int Id, string Name, TimeSpan StartTime, TimeSpan EndTime, int GraceMinutes, bool IsDefault);
public sealed record CreateWorkScheduleRequest(string Name, TimeSpan StartTime, TimeSpan EndTime, int GraceMinutes, bool IsDefault = false);

public sealed class WorkScheduleHandler(IRepository<WorkSchedule> repository, IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyCollection<WorkScheduleDto>> GetAsync(CancellationToken ct) =>
        await repository.QueryAsync(x => new WorkScheduleDto(x.Id, x.Name, x.StartTime, x.EndTime, x.GraceMinutes, x.IsDefault), take: 500, cancellationToken: ct);

    public async Task<int> CreateAsync(CreateWorkScheduleRequest request, CancellationToken ct)
    {
        var schedule = new WorkSchedule(request.Name, request.StartTime, request.EndTime, request.GraceMinutes, request.IsDefault);
        await repository.AddAsync(schedule, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return schedule.Id;
    }
}
