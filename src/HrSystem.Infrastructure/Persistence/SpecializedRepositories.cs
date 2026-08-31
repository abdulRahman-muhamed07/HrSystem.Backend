using HrSystem.Domain.Entities;
using HrSystem.Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Persistence;

public sealed class EmployeeRepository(AppDbContext db) : EfRepository<Employee>(db), IEmployeeRepository
{
    public Task<Employee?> GetWithDepartmentAsync(int id, CancellationToken ct = default) => Query().AsNoTracking().Include(e => e.Department).SingleOrDefaultAsync(e => e.Id == id, ct);
    public Task<bool> EmailExistsAsync(string email, int? excludingId = null, CancellationToken ct = default) => Query().AnyAsync(e => e.Email == email && (!excludingId.HasValue || e.Id != excludingId.Value), ct);
    public async Task<IReadOnlyList<Employee>> SearchAsync(string? term, int page, int pageSize, CancellationToken ct = default)
    {
        IQueryable<Employee> query = Query().AsNoTracking().Include(e => e.Department).OrderBy(e => e.FullName);
        if (!string.IsNullOrWhiteSpace(term)) query = query.Where(e => e.FullName.Contains(term) || e.Email.Contains(term));
        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }
}

public sealed class LeaveRepository(AppDbContext db) : EfRepository<LeaveRequest>(db), ILeaveRepository
{
    public Task<bool> HasOverlapAsync(int employeeId, DateTime start, DateTime end, int? excludingId = null, CancellationToken ct = default) =>
        Query().AnyAsync(l => l.EmployeeId == employeeId && (!excludingId.HasValue || l.Id != excludingId.Value) && l.StartDate <= end && l.EndDate >= start && l.Status.ToString() == "Approved", ct);
}

public sealed class PayrollRepository(AppDbContext db) : EfRepository<PayrollRecord>(db), IPayrollRepository
{
    public Task<PayrollRecord?> GetForPeriodAsync(int employeeId, int year, int month, CancellationToken ct = default) => Query().SingleOrDefaultAsync(p => p.EmployeeId == employeeId && p.Year == year && p.Month == month, ct);
}
