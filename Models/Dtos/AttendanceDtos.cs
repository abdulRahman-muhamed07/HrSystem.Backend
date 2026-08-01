namespace HrSystem.Backend.Models.Dtos;

public class AttendanceDto
{
    public int Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string? CheckIn { get; set; }
    public string? CheckOut { get; set; }
    public string Status { get; set; } = string.Empty;
}