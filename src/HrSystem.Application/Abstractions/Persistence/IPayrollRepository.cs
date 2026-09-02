using HrSystem.Domain.Entities;

namespace HrSystem.Application.Abstractions.Persistence;

public interface IPayrollRepository : IRepository<PayrollRecord>
{
    Task<PayrollRecord?> GetForPeriodAsync(int employeeId, int year, int month, CancellationToken cancellationToken = default);
}
