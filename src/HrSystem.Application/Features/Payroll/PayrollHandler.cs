using HrSystem.Application;
using HrSystem.Application.Models.Payroll;

namespace HrSystem.Application.Features.Payroll;

public sealed class PayrollHandler(IPayrollService service)
{
    public Task<PayrollDto> GenerateAsync(int employeeId, int year, int month, CancellationToken ct) => service.GenerateAsync(employeeId, year, month, ct);
    public Task<IReadOnlyCollection<PayrollDto>> GetMonthAsync(int year, int month, CancellationToken ct) => service.GetMonthAsync(year, month, ct);
    public Task PayAsync(int id, CancellationToken ct) => service.PayAsync(id, ct);
}
