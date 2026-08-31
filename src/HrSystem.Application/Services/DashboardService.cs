using HrSystem.Domain.Entities;
using HrSystem.Domain.Enums;

namespace HrSystem.Application.Services;

public sealed class DashboardService(
    IRepository<Employee> employees,
    IRepository<LeaveRequest> leaves,
    IRepository<OvertimeRequest> overtime,
    IRepository<EmployeeLoan> loans,
    IRepository<PayrollRecord> payroll) : IDashboardService
{
    public async Task<DashboardDto> GetAsync(int year, int month, CancellationToken ct)
    {
        if (month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");

        var employeeCountTask = employees.CountAsync(null, ct);
        var activeEmployeeTask = employees.CountAsync(e => e.EmploymentStatus == EmploymentStatus.Active, ct);
        var pendingLeavesTask = leaves.CountAsync(l => l.Status == LeaveRequestStatus.Pending, ct);
        var pendingOvertimeTask = overtime.CountAsync(o => o.Status == OvertimeStatus.Pending, ct);
        var pendingLoansTask = loans.CountAsync(l => l.Status == LoanStatus.Pending, ct);
        var payrollNetTask = payroll.QueryAsync(
            p => p.NetSalary,
            p => p.Year == year && p.Month == month,
            0,
            int.MaxValue,
            ct);

        await Task.WhenAll(employeeCountTask, activeEmployeeTask, pendingLeavesTask, pendingOvertimeTask, pendingLoansTask, payrollNetTask);

        return new DashboardDto(
            employeeCountTask.Result,
            activeEmployeeTask.Result,
            pendingLeavesTask.Result,
            pendingOvertimeTask.Result,
            pendingLoansTask.Result,
            payrollNetTask.Result.Sum());
    }
}
