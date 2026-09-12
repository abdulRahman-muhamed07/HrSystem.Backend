namespace HrSystem.Domain.Entities;

public sealed class Holiday
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }
    public bool IsPaid { get; private set; } = true;
    private Holiday() { }
    public Holiday(string name, DateTime date, bool isPaid = true)
    { Name = name.Trim(); Date = date.Date; IsPaid = isPaid; }
}
