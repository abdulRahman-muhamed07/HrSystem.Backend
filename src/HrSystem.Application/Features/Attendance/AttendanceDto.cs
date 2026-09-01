using HrSystem.Domain.Enums;

namespace HrSystem.Application.Models.Attendance;

public sealed record AttendanceDto(int Id, int EmployeeId, DateTime Date, TimeOnly? CheckIn, TimeOnly? CheckOut, AttendanceStatus Status);
