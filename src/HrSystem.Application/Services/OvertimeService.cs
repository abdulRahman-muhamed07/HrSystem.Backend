using HrSystem.Application.Exceptions;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Services;

public sealed class OvertimeService(IRepository<OvertimeRequest> overtime, IRepository<Employee> employees, IUnitOfWork unitOfWork, IAuditService audit, ICurrentUser currentUser) : IOvertimeService
{
    public async Task<int> CreateAsync(CreateOvertimeRequest request, CancellationToken ct)
    {
        if (request.Hours <= 0 || request.Hours > 24) throw new BusinessRuleException("Overtime hours must be between 0 and 24.");
        if (request.RateMultiplier <= 0) throw new BusinessRuleException("Rate multiplier must be positive.");
        if (await employees.GetByIdAsync(request.EmployeeId, ct) is null) throw new NotFoundException("Employee was not found.");
        var entity = new OvertimeRequest(request.EmployeeId, request.Date, request.Hours, request.RateMultiplier, request.Reason);
        await overtime.AddAsync(entity, ct); await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Create", nameof(OvertimeRequest), entity.Id.ToString(), null, ct); return entity.Id;
    }
    public async Task<IReadOnlyCollection<OvertimeDto>> GetPendingAsync(CancellationToken ct) => await overtime.QueryAsync(o => new OvertimeDto(o.Id, o.EmployeeId, o.Employee!.FullName, o.Date, o.Hours, o.RateMultiplier, o.Reason, o.Status), o => o.Status == HrSystem.Domain.Enums.OvertimeStatus.Pending, 0, int.MaxValue, ct);
    public async Task DecideAsync(int id, bool approve, CancellationToken ct)
    {
        var entity = await overtime.GetByIdAsync(id, ct) ?? throw new NotFoundException("Overtime request was not found.");
        if (entity.Status != HrSystem.Domain.Enums.OvertimeStatus.Pending) throw new BusinessRuleException("Only pending overtime requests can be decided.");
        var userId = currentUser.UserId ?? throw new BusinessRuleException("Authenticated user is required.");
        if (approve) entity.Approve(userId); else entity.Reject(userId);
        await unitOfWork.SaveChangesAsync(ct); await audit.WriteAsync(approve ? "Approve" : "Reject", nameof(OvertimeRequest), id.ToString(), null, ct);
    }
}
