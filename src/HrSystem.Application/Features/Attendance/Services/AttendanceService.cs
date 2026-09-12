using AutoMapper;
using HrSystem.Application.Exceptions;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Enums;

namespace HrSystem.Application.Services;

public sealed class AttendanceService(
    IRepository<AttendanceRecord> attendance,
    IRepository<Employee> employees,
    IUnitOfWork unitOfWork,
    IAuditService audit,
    IMapper mapper,
    ICurrentUser currentUser) : IAttendanceService
{
    private bool IsHrOrAdmin() => currentUser.Role is nameof(UserRole.Admin) or nameof(UserRole.HR);

    private void EnsureEmployeeAccess(int employeeId)
    {
        if (!IsHrOrAdmin() && currentUser.EmployeeId != employeeId)
            throw new BusinessRuleException("You can only access your own attendance records.");
    }

    public async Task<AttendanceDto> CheckInAsync(CheckInRequest request, CancellationToken ct)
    {
        EnsureEmployeeAccess(request.EmployeeId);

        if (await employees.GetByIdAsync(request.EmployeeId, ct) is null)
            throw new NotFoundException("Employee was not found.");

        var nowUtc = DateTime.UtcNow;
        var currentDate = nowUtc.Date;
        var now = request.CheckIn ?? TimeOnly.FromDateTime(nowUtc);
        var existing = (await attendance.QueryAsync(
            a => a,
            a => a.EmployeeId == request.EmployeeId && a.Date == currentDate,
            0,
            1,
            ct)).FirstOrDefault();

        if (existing is not null)
        {
            if (existing.CheckIn.HasValue)
                throw new BusinessRuleException("Employee has already checked in today.");

            existing.CheckInAt(now, now > new TimeOnly(9, 0) ? AttendanceStatus.Late : AttendanceStatus.OnTime);
            await unitOfWork.SaveChangesAsync(ct);
            await audit.WriteAsync("CheckIn", nameof(AttendanceRecord), existing.Id.ToString(), $"Employee {request.EmployeeId} checked in.", ct);
            return mapper.Map<AttendanceDto>(existing);
        }

        var record = new AttendanceRecord(request.EmployeeId, currentDate, now);
        record.CheckInAt(now, now > new TimeOnly(9, 0) ? AttendanceStatus.Late : AttendanceStatus.OnTime);
        await attendance.AddAsync(record, ct);
        await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("CheckIn", nameof(AttendanceRecord), record.Id.ToString(), $"Employee {request.EmployeeId} checked in.", ct);

        return mapper.Map<AttendanceDto>(record);
    }

    public async Task<AttendanceDto> CheckOutAsync(int id, CheckOutRequest request, CancellationToken ct)
    {
        var record = await attendance.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Attendance record was not found.");

        EnsureEmployeeAccess(record.EmployeeId);

        if (!record.CheckIn.HasValue)
            throw new BusinessRuleException("Employee must check in before checking out.");
        if (record.CheckOut.HasValue)
            throw new BusinessRuleException("Attendance record has already been checked out.");

        record.CheckOutAt(request.CheckOut ?? TimeOnly.FromDateTime(DateTime.UtcNow));
        await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("CheckOut", nameof(AttendanceRecord), id.ToString(), $"Employee {record.EmployeeId} checked out.", ct);

        return mapper.Map<AttendanceDto>(record);
    }

    public async Task<PagedResult<AttendanceDto>> GetPagedAsync(int page, int pageSize, int? employeeId, CancellationToken ct)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        if (!IsHrOrAdmin())
            employeeId = currentUser.EmployeeId;

        if (!employeeId.HasValue)
            throw new BusinessRuleException("An employee context is required.");

        var predicate = (System.Linq.Expressions.Expression<Func<AttendanceRecord, bool>>)(a => a.EmployeeId == employeeId.Value);
        var total = await attendance.CountAsync(predicate, ct);
        var entities = await attendance.QueryAsync(
            a => a,
            predicate,
            (page - 1) * pageSize,
            pageSize,
            ct);

        return new(mapper.Map<List<AttendanceDto>>(entities), page, pageSize, total);
    }
}
