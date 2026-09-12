namespace HrSystem.Domain.Entities;

public sealed class PayrollRule
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Value { get; private set; }
    public bool IsPercentage { get; private set; }
    public bool IsActive { get; private set; } = true;
    private PayrollRule() { }
    public PayrollRule(string name, decimal value, bool isPercentage)
    { Name = name.Trim(); Value = value; IsPercentage = isPercentage; }
}
