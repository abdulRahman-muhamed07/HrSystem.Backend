namespace HrSystem.Backend.Models.Dtos;

public class DashboardStatsDto
{
    public int TotalEmployees { get; set; }
    public int TotalDepartments { get; set; }
    public int PendingLeaves { get; set; }
    public double AttendanceRate { get; set; }
    public int PresentToday { get; set; }
    public int TotalCheckedIn { get; set; }

    // Extended metrics
    public int ActiveEmployees { get; set; }
    public int PendingOvertime { get; set; }
    public int PendingLoans { get; set; }
    public decimal TotalMonthlyPayroll { get; set; }
}
