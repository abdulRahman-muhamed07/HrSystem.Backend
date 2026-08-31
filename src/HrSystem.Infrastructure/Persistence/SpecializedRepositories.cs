using HrSystem.Application;
using HrSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Persistence;

public sealed class EmployeeRepository(AppDbContext db) : EfRepository<Employee>(db), IEmployeeRepository
{
    public Task<bool> EmailExistsAsync(string email, int? excludingId = null, CancellationToken cancellationToken = default) =>
        Query().AsNoTracking().AnyAsync(e => e.Email == email && (!excludingId.HasValue || e.Id != excludingId.Value), cancellationToken);

    public Task<Employee?> GetWithDepartmentAsync(int id, CancellationToken cancellationToken = default) =>
        Query().AsNoTracking().Include(e => e.Department).SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Employee>> SearchAsync(string? term, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        IQueryable<Employee> query = Query().AsNoTracking().Include(e => e.Department).OrderBy(e => e.FullName);
        if (!string.IsNullOrWhiteSpace(term))
            query = query.Where(e => e.FullName.Contains(term) || e.Email.Contains(term));

        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }
}

public sealed class LeaveRepository(AppDbContext db) : EfRepository<LeaveRequest>(db), ILeaveRepository
{
    public Task<bool> HasOverlapAsync(int employeeId, DateTime start, DateTime end, int? excludingId = null, CancellationToken cancellationToken = default) =>
        Query().AsNoTracking().AnyAsync(
            l => l.EmployeeId == employeeId
                 && (!excludingId.HasValue || l.Id != excludingId.Value)
                 && l.StartDate <= end
                 && l.EndDate >= start
                 && l.Status == HrSystem.Domain.Enums.LeaveRequestStatus.Approved,
            cancellationToken);
}

public sealed class PayrollRepository(AppDbContext db) : EfRepository<PayrollRecord>(db), IPayrollRepository
{
    public Task<PayrollRecord?> GetForPeriodAsync(int employeeId, int year, int month, CancellationToken cancellationToken = default) =>
        Query().SingleOrDefaultAsync(p => p.EmployeeId == employeeId && p.Year == year && p.Month == month, cancellationToken);
}
