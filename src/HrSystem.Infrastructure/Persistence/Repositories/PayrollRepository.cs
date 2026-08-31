using HrSystem.Application;
using HrSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Persistence.Repositories;

public sealed class PayrollRepository(AppDbContext db) : EfRepository<PayrollRecord>(db), IPayrollRepository
{
    public Task<PayrollRecord?> GetForPeriodAsync(int employeeId, int year, int month, CancellationToken cancellationToken = default) =>
        Query().SingleOrDefaultAsync(p => p.EmployeeId == employeeId && p.Year == year && p.Month == month, cancellationToken);
}
