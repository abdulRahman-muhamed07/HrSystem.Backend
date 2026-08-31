using HrSystem.Application;
using HrSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Persistence.Repositories;

public sealed class DepartmentRepository(AppDbContext db) : EfRepository<Department>(db), IDepartmentRepository
{
    public Task<List<Department>> GetAllWithEmployeesAsync(CancellationToken cancellationToken = default) =>
        Query().AsNoTracking().Include(d => d.Employees).OrderBy(d => d.Name).ToListAsync(cancellationToken);
}
