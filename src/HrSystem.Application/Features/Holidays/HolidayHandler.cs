using HrSystem.Application;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Features.Holidays;

public sealed record HolidayDto(int Id, string Name, DateTime Date, bool IsPaid);
public sealed record CreateHolidayRequest(string Name, DateTime Date, bool IsPaid = true);

public sealed class HolidayHandler(IRepository<Holiday> repository, IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyCollection<HolidayDto>> GetAsync(int year, CancellationToken ct) =>
        await repository.QueryAsync(x => new HolidayDto(x.Id, x.Name, x.Date, x.IsPaid), x => x.Date.Year == year, take: 500, cancellationToken: ct);

    public async Task<int> CreateAsync(CreateHolidayRequest request, CancellationToken ct)
    {
        var holiday = new Holiday(request.Name, request.Date, request.IsPaid);
        await repository.AddAsync(holiday, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return holiday.Id;
    }
}
