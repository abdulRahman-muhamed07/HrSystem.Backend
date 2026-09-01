using HrSystem.Application.Models.Attendance;
using HrSystem.Application.Models.Common;

namespace HrSystem.Application.Features.Attendance.Contracts;

public interface IAttendanceService
{
    Task<AttendanceDto> CheckInAsync(CheckInRequest request, CancellationToken ct);
    Task<AttendanceDto> CheckOutAsync(int id, CheckOutRequest request, CancellationToken ct);
    Task<PagedResult<AttendanceDto>> GetPagedAsync(int page, int pageSize, int? employeeId, CancellationToken ct);
}
