namespace HrSystem.Domain.Entities;

public sealed class WorkSchedule
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public int GraceMinutes { get; private set; }
    public bool IsDefault { get; private set; }
    private WorkSchedule() { }
    public WorkSchedule(string name, TimeSpan startTime, TimeSpan endTime, int graceMinutes, bool isDefault = false)
    { Name = name.Trim(); StartTime = startTime; EndTime = endTime; GraceMinutes = Math.Max(0, graceMinutes); IsDefault = isDefault; }
}
