using HrSystem.Application;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Persistence;

public sealed class EfRepository<T>(AppDbContext db) : IRepository<T> where T : class
{
    private readonly DbSet<T> _set = db.Set<T>();
    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => _set.FindAsync([id], cancellationToken).AsTask();
    public Task<List<TResult>> QueryAsync<TResult>(System.Linq.Expressions.Expression<Func<T, TResult>> selector, System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null, int skip = 0, int take = int.MaxValue, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _set.AsNoTracking();
        if (predicate is not null) query = query.Where(predicate);
        return query.OrderBy(e => EF.Property<object>(e, "Id")).Skip(skip).Take(take).Select(selector).ToListAsync(cancellationToken);
    }
    public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => predicate is null ? _set.CountAsync(cancellationToken) : _set.CountAsync(predicate, cancellationToken);
    public Task AddAsync(T entity, CancellationToken cancellationToken = default) => _set.AddAsync(entity, cancellationToken).AsTask();
    public void Remove(T entity) => _set.Remove(entity);
}
