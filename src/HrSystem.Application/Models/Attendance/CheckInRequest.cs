namespace HrSystem.Application.Models.Attendance;

public sealed record CheckInRequest(int EmployeeId, TimeOnly? CheckIn = null);
