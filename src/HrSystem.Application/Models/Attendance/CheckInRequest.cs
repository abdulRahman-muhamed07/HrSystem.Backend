namespace HrSystem.Application;

public sealed record CheckInRequest(int EmployeeId, TimeOnly? CheckIn = null);
