using HrSystem.Domain.Enums;

namespace HrSystem.Application;

public sealed record AttendanceDto(int Id, int EmployeeId, DateTime Date, TimeOnly? CheckIn, TimeOnly? CheckOut, AttendanceStatus Status);
