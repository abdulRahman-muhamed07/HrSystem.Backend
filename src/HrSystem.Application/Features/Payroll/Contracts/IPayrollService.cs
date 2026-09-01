namespace HrSystem.Application;

public interface IPayrollService
{
    Task<PayrollDto> GenerateAsync(int employeeId, int year, int month, CancellationToken ct);
    Task<IReadOnlyCollection<PayrollDto>> GetMonthAsync(int year, int month, CancellationToken ct);
    Task PayAsync(int id, CancellationToken ct);
}
