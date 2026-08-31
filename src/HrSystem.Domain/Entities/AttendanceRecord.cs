using HrSystem.Domain.Enums;

namespace HrSystem.Domain.Entities;

public sealed class AttendanceRecord
{
    public int Id { get; private set; }
    public int EmployeeId { get; private set; }
    public DateTime Date { get; private set; }
    public TimeOnly? CheckIn { get; private set; }
    public TimeOnly? CheckOut { get; private set; }
    public AttendanceStatus Status { get; private set; } = AttendanceStatus.OnTime;
    public Employee? Employee { get; private set; }

    private AttendanceRecord() { }
    public AttendanceRecord(int employeeId, DateTime date, TimeOnly? checkIn = null)
    {
        EmployeeId = employeeId; Date = date.Date; CheckIn = checkIn; Status = checkIn is null ? AttendanceStatus.Absent : AttendanceStatus.OnTime;
    }
    public void CheckInAt(TimeOnly time, AttendanceStatus status = AttendanceStatus.OnTime) { CheckIn = time; Status = status; }
    public void CheckOutAt(TimeOnly time) => CheckOut = time;
}
