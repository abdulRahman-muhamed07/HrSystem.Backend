using HrSystem.Application;
using HrSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Persistence.Repositories;

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
            query = query.Where(e => e.FullName.Contains(term) || e.Email.Contains(term) || e.JobTitle.Contains(term));

        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }

    public Task<int> CountSearchAsync(string? term, CancellationToken cancellationToken = default)
        => string.IsNullOrWhiteSpace(term)
            ? Query().CountAsync(cancellationToken)
            : Query().CountAsync(e => e.FullName.Contains(term) || e.Email.Contains(term) || e.JobTitle.Contains(term), cancellationToken);
}
