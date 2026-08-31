using HrSystem.Domain.Enums;

namespace HrSystem.Domain.Entities;

public sealed class PayrollRecord
{
    public int Id { get; private set; }
    public int EmployeeId { get; private set; }
    public int Year { get; private set; }
    public int Month { get; private set; }
    public decimal BasicSalary { get; private set; }
    public decimal HousingAllowance { get; private set; }
    public decimal TransportationAllowance { get; private set; }
    public decimal MealAllowance { get; private set; }
    public decimal OtherAllowances { get; private set; }
    public decimal OvertimePay { get; private set; }
    public decimal GrossSalary { get; private set; }
    public decimal GosiEmployee { get; private set; }
    public decimal GosiEmployer { get; private set; }
    public decimal IncomeTax { get; private set; }
    public decimal LoanDeduction { get; private set; }
    public decimal OtherDeductions { get; private set; }
    public decimal NetSalary { get; private set; }
    public PayrollStatus Status { get; private set; } = PayrollStatus.Draft;
    public DateTime? PaidAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Employee? Employee { get; private set; }
    private PayrollRecord() { }
    public PayrollRecord(int employeeId, int year, int month) { EmployeeId = employeeId; Year = year; Month = month; }
    public void Calculate(decimal basic, decimal housing, decimal transport, decimal meal, decimal other, decimal overtime, decimal gosiEmployee, decimal gosiEmployer, decimal tax, decimal loan, decimal otherDeductions)
    {
        BasicSalary = basic; HousingAllowance = housing; TransportationAllowance = transport; MealAllowance = meal; OtherAllowances = other; OvertimePay = overtime;
        GrossSalary = basic + housing + transport + meal + other + overtime; GosiEmployee = gosiEmployee; GosiEmployer = gosiEmployer; IncomeTax = tax; LoanDeduction = loan; OtherDeductions = otherDeductions;
        NetSalary = GrossSalary - gosiEmployee - tax - loan - otherDeductions;
    }
    public void MarkPaid() { Status = PayrollStatus.Paid; PaidAt = DateTime.UtcNow; }
}
