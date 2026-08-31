using AutoMapper;
using HrSystem.Application.Exceptions;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Enums;

namespace HrSystem.Application.Services;

public sealed class LeaveService(
    ILeaveRepository leaves,
    IRepository<Employee> employees,
    IRepository<LeaveType> leaveTypes,
    IRepository<EmployeeLeaveBalance> balances,
    IUnitOfWork unitOfWork,
    IAuditService audit,
    ICurrentUser currentUser,
    IMapper mapper) : ILeaveService
{
    public async Task<int> CreateAsync(CreateLeaveRequest request, CancellationToken ct)
    {
        if (request.EndDate.Date < request.StartDate.Date)
            throw new BusinessRuleException("End date cannot be before start date.");

        if (await employees.GetByIdAsync(request.EmployeeId, ct) is null)
            throw new NotFoundException("Employee was not found.");

        var type = await leaveTypes.GetByIdAsync(request.LeaveTypeId, ct)
            ?? throw new NotFoundException("Leave type was not found.");

        if (!type.IsActive)
            throw new BusinessRuleException("Leave type is inactive.");

        if (await leaves.HasOverlapAsync(request.EmployeeId, request.StartDate.Date, request.EndDate.Date, cancellationToken: ct))
            throw new BusinessRuleException("The leave dates overlap an existing leave request.");

        var duration = WorkingDays(request.StartDate, request.EndDate);
        if (duration <= 0)
            throw new BusinessRuleException("Leave request must contain at least one working day.");

        var year = request.StartDate.Year;
        var balance = (await balances.QueryAsync(
            b => b,
            b => b.EmployeeId == request.EmployeeId && b.LeaveTypeId == request.LeaveTypeId && b.Year == year,
            0,
            1,
            ct)).FirstOrDefault();

        if (balance is null)
        {
            balance = new EmployeeLeaveBalance(request.EmployeeId, request.LeaveTypeId, year, type.DaysPerYear);
            await balances.AddAsync(balance, ct);
        }

        if (balance.AvailableDays < duration)
            throw new BusinessRuleException("Insufficient leave balance.");

        var leave = new LeaveRequest(request.EmployeeId, request.LeaveTypeId, request.StartDate, request.EndDate, duration, request.Reason);
        await leaves.AddAsync(leave, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Create", nameof(LeaveRequest), leave.Id.ToString(), $"Created leave request for employee {request.EmployeeId}.", ct);

        return leave.Id;
    }

    public async Task<PagedResult<LeaveRequestDto>> GetPagedAsync(int page, int pageSize, LeaveRequestStatus? status, CancellationToken ct)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var predicate = status.HasValue
            ? (System.Linq.Expressions.Expression<Func<LeaveRequest, bool>>)(l => l.Status == status.Value)
            : null;

        var total = await leaves.CountAsync(predicate, ct);
        var entities = await leaves.QueryAsync(
            l => l,
            predicate,
            (page - 1) * pageSize,
            pageSize,
            ct);

        return new(mapper.Map<List<LeaveRequestDto>>(entities), page, pageSize, total);
    }

    public async Task DecideAsync(int id, LeaveDecisionRequest request, CancellationToken ct)
    {
        var leave = await leaves.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Leave request was not found.");

        if (leave.Status != LeaveRequestStatus.Pending)
            throw new BusinessRuleException("Only pending leave requests can be decided.");

        var userId = currentUser.UserId
            ?? throw new BusinessRuleException("Authenticated user is required.");

        var balance = (await balances.QueryAsync(
            b => b,
            b => b.EmployeeId == leave.EmployeeId && b.LeaveTypeId == leave.LeaveTypeId && b.Year == leave.StartDate.Year,
            0,
            1,
            ct)).FirstOrDefault();

        if (request.Approve)
        {
            if (balance is null || balance.AvailableDays < leave.DurationDays)
                throw new BusinessRuleException("Insufficient leave balance.");

            leave.Approve(userId);
            balance.AddUsage(leave.DurationDays);
        }
        else
        {
            leave.Reject(userId, string.IsNullOrWhiteSpace(request.RejectionReason) ? "Rejected by HR." : request.RejectionReason);
        }

        await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync(request.Approve ? "Approve" : "Reject", nameof(LeaveRequest), id.ToString(), null, ct);
    }

    private static decimal WorkingDays(DateTime start, DateTime end)
    {
        var days = 0;
        for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
        {
            if (date.DayOfWeek is not DayOfWeek.Friday and not DayOfWeek.Saturday)
                days++;
        }

        return days;
    }
}
