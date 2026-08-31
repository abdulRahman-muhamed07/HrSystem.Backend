using HrSystem.Application;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure.Persistence;

public class EfRepository<T>(AppDbContext db) : IRepository<T> where T : class
{
    protected readonly DbSet<T> Set = db.Set<T>();

    protected IQueryable<T> Query() => Set;

    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        Set.FindAsync([id], cancellationToken).AsTask();

    public Task<List<TResult>> QueryAsync<TResult>(
        System.Linq.Expressions.Expression<Func<T, TResult>> selector,
        System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null,
        int skip = 0,
        int take = int.MaxValue,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = Set.AsNoTracking();
        if (predicate is not null)
            query = query.Where(predicate);

        return query
            .OrderBy(e => EF.Property<object>(e, "Id"))
            .Skip(skip)
            .Take(take)
            .Select(selector)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(
        System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null ? Set.CountAsync(cancellationToken) : Set.CountAsync(predicate, cancellationToken);

    public Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        Set.AddAsync(entity, cancellationToken).AsTask();

    public void Remove(T entity) => Set.Remove(entity);
}
