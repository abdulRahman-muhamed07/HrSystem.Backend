namespace HrSystem.Backend.Models;

public class AttendanceRecord
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public TimeOnly? CheckIn { get; set; }
    public TimeOnly? CheckOut { get; set; }
    public string Status { get; set; } = "OnTime"; // OnTime, Late, Absent

    // Navigation
    public virtual Employee? Employee { get; set; }
}