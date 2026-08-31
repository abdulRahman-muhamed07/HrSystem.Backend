using HrSystem.Domain.Entities;
using HrSystem.Domain.Enums;

namespace HrSystem.Application.Services;

public sealed class DashboardService(IRepository<Employee> employees, IRepository<LeaveRequest> leaves, IRepository<OvertimeRequest> overtime, IRepository<EmployeeLoan> loans, IRepository<PayrollRecord> payroll) : IDashboardService
{
    public async Task<DashboardDto> GetAsync(int year, int month, CancellationToken ct)
    {
        var employeeCount = await employees.CountAsync(null, ct);
        var active = await employees.CountAsync(e => e.EmploymentStatus == EmploymentStatus.Active, ct);
        var pendingLeaves = await leaves.CountAsync(l => l.Status == LeaveRequestStatus.Pending, ct);
        var pendingOvertime = await overtime.CountAsync(o => o.Status == OvertimeStatus.Pending, ct);
        var pendingLoans = await loans.CountAsync(l => l.Status == LoanStatus.Pending, ct);
        var net = (await payroll.QueryAsync(p => p.NetSalary, p => p.Year == year && p.Month == month, 0, int.MaxValue, ct)).Sum();
        return new(employeeCount, active, pendingLeaves, pendingOvertime, pendingLoans, net);
    }
}
