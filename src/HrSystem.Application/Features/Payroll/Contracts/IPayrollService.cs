using HrSystem.Application.Models.Payroll;

namespace HrSystem.Application.Features.Payroll.Contracts;

public interface IPayrollService
{
    Task<PayrollDto> GenerateAsync(int employeeId, int year, int month, CancellationToken ct);
    Task<IReadOnlyCollection<PayrollDto>> GetMonthAsync(int year, int month, CancellationToken ct);
    Task PayAsync(int id, CancellationToken ct);
}
