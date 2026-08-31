using HrSystem.Application.Exceptions;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Enums;

namespace HrSystem.Application.Services;

public sealed class PayrollService(IRepository<PayrollRecord> payroll, IRepository<Employee> employees, IRepository<OvertimeRequest> overtime, IRepository<EmployeeLoan> loans, IUnitOfWork unitOfWork, IAuditService audit) : IPayrollService
{
    public async Task<PayrollDto> GenerateAsync(int employeeId, int year, int month, CancellationToken ct)
    {
        if (month is < 1 or > 12) throw new BusinessRuleException("Month must be between 1 and 12.");
        var employee = await employees.GetByIdAsync(employeeId, ct) ?? throw new NotFoundException("Employee was not found.");
        var existing = (await payroll.QueryAsync(p => p, p => p.EmployeeId == employeeId && p.Year == year && p.Month == month, 0, 1, ct)).FirstOrDefault();
        if (existing is not null) return Map(existing, employee.FullName);

        var overtimeHours = (await overtime.QueryAsync(o => new { o.Hours, o.RateMultiplier }, o => o.EmployeeId == employeeId && o.Status == OvertimeStatus.Approved && o.Date.Year == year && o.Date.Month == month, 0, int.MaxValue, ct)).ToList();
        var overtimePay = overtimeHours.Sum(x => employee.Salary / 22m / 8m * x.Hours * x.RateMultiplier);
        var loanDeduction = (await loans.QueryAsync(l => l.MonthlyDeduction, l => l.EmployeeId == employeeId && l.Status == LoanStatus.Approved && l.RemainingAmount > 0, 0, int.MaxValue, ct)).Sum();
        var gross = employee.Salary + employee.HousingAllowance + employee.TransportationAllowance + employee.MealAllowance + overtimePay;
        var gosiEmployee = Math.Round(gross * 0.10m, 2);
        var record = new PayrollRecord(employeeId, year, month);
        record.Calculate(employee.Salary, employee.HousingAllowance, employee.TransportationAllowance, employee.MealAllowance, 0, overtimePay, gosiEmployee, 0, 0, Math.Min(loanDeduction, gross - gosiEmployee), 0);
        await payroll.AddAsync(record, ct); await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Generate", nameof(PayrollRecord), record.Id.ToString(), $"Generated payroll for employee {employeeId} for {month}/{year}.", ct);
        return Map(record, employee.FullName);
    }

    public async Task<IReadOnlyCollection<PayrollDto>> GetMonthAsync(int year, int month, CancellationToken ct) => await payroll.QueryAsync(p => new PayrollDto(p.Id, p.EmployeeId, p.Employee!.FullName, p.Year, p.Month, p.GrossSalary, p.NetSalary, p.OvertimePay, p.LoanDeduction, p.Status, p.PaidAt), p => p.Year == year && p.Month == month, 0, int.MaxValue, ct);

    public async Task PayAsync(int id, CancellationToken ct)
    {
        var record = await payroll.GetByIdAsync(id, ct) ?? throw new NotFoundException("Payroll record was not found.");
        if (record.Status == PayrollStatus.Paid) throw new BusinessRuleException("Payroll record is already paid.");
        record.MarkPaid(); await unitOfWork.SaveChangesAsync(ct);
        await audit.WriteAsync("Pay", nameof(PayrollRecord), id.ToString(), null, ct);
    }

    private static PayrollDto Map(PayrollRecord p, string name) => new(p.Id, p.EmployeeId, name, p.Year, p.Month, p.GrossSalary, p.NetSalary, p.OvertimePay, p.LoanDeduction, p.Status, p.PaidAt);
}
