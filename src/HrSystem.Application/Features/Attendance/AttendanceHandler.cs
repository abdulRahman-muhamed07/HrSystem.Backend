using HrSystem.Application;
using HrSystem.Application.Models.Attendance;
using HrSystem.Application.Models.Common;

namespace HrSystem.Application.Features.Attendance;

public sealed class AttendanceHandler(IAttendanceService service)
{
    public Task<PagedResult<AttendanceDto>> GetAsync(int page, int pageSize, int? employeeId, CancellationToken ct) => service.GetPagedAsync(page, pageSize, employeeId, ct);
    public Task<AttendanceDto> CheckInAsync(CheckInRequest request, CancellationToken ct) => service.CheckInAsync(request, ct);
    public Task<AttendanceDto> CheckOutAsync(int id, CheckOutRequest request, CancellationToken ct) => service.CheckOutAsync(id, request, ct);
}
