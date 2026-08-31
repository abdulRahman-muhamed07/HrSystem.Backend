using HrSystem.Application.Exceptions;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Enums;

namespace HrSystem.Application.Services;

public sealed class AttendanceService(IRepository<AttendanceRecord> attendance, IRepository<Employee> employees, IUnitOfWork unitOfWork, IAuditService audit) : IAttendanceService
{
    public async Task<AttendanceDto> CheckInAsync(CheckInRequest request, CancellationToken ct)
    {
        if (await employees.GetByIdAsync(request.EmployeeId, ct) is null) throw new NotFoundException("Employee was not found.");
        var now = request.CheckIn ?? TimeOnly.FromDateTime(DateTime.Now);
        var existing = (await attendance.QueryAsync(a => a, a => a.EmployeeId == request.EmployeeId && a.Date == DateTime.UtcNow.Date, 0, 1, ct)).FirstOrDefault();
        if (existing is not null) { existing.CheckInAt(now, now > new TimeOnly(9, 0) ? AttendanceStatus.Late : AttendanceStatus.OnTime); await unitOfWork.SaveChangesAsync(ct); return Map(existing); }
        var record = new AttendanceRecord(request.EmployeeId, DateTime.UtcNow.Date, now);
        record.CheckInAt(now, now > new TimeOnly(9, 0) ? AttendanceStatus.Late : AttendanceStatus.OnTime);
        await attendance.AddAsync(record, ct); await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("CheckIn", nameof(AttendanceRecord), record.Id.ToString(), $"Employee {request.EmployeeId} checked in.", ct);
        return Map(record);
    }

    public async Task<AttendanceDto> CheckOutAsync(int id, CheckOutRequest request, CancellationToken ct)
    {
        var record = await attendance.GetByIdAsync(id, ct) ?? throw new NotFoundException("Attendance record was not found.");
        record.CheckOutAt(request.CheckOut ?? TimeOnly.FromDateTime(DateTime.Now)); await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("CheckOut", nameof(AttendanceRecord), id.ToString(), $"Employee {record.EmployeeId} checked out.", ct);
        return Map(record);
    }

    public async Task<PagedResult<AttendanceDto>> GetPagedAsync(int page, int pageSize, int? employeeId, CancellationToken ct)
    {
        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 1, 100);
        var predicate = employeeId.HasValue ? (System.Linq.Expressions.Expression<Func<AttendanceRecord, bool>>)(a => a.EmployeeId == employeeId.Value) : null;
        var total = await attendance.CountAsync(predicate, ct);
        var items = await attendance.QueryAsync(a => new AttendanceDto(a.Id, a.EmployeeId, a.Date, a.CheckIn, a.CheckOut, a.Status), predicate, (page - 1) * pageSize, pageSize, ct);
        return new(items, page, pageSize, total);
    }

    private static AttendanceDto Map(AttendanceRecord a) => new(a.Id, a.EmployeeId, a.Date, a.CheckIn, a.CheckOut, a.Status);
}
