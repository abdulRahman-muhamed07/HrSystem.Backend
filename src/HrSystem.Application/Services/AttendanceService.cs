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
    IMapper mapper) : IAttendanceService
{
    public async Task<AttendanceDto> CheckInAsync(CheckInRequest request, CancellationToken ct)
    {
        if (await employees.GetByIdAsync(request.EmployeeId, ct) is null)
            throw new NotFoundException("Employee was not found.");

        var currentDate = DateTime.UtcNow.Date;
        var now = request.CheckIn ?? TimeOnly.FromDateTime(DateTime.UtcNow);
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
        var predicate = employeeId.HasValue
            ? (System.Linq.Expressions.Expression<Func<AttendanceRecord, bool>>)(a => a.EmployeeId == employeeId.Value)
            : null;

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
