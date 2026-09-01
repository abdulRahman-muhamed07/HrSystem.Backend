using System.Linq.Expressions;
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
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        int skip = 0,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        if (skip < 0) throw new ArgumentOutOfRangeException(nameof(skip));
        if (take <= 0) throw new ArgumentOutOfRangeException(nameof(take));

        IQueryable<T> query = Set.AsNoTracking();
        if (predicate is not null)
            query = query.Where(predicate);

        return query
            .Skip(skip)
            .Take(take)
            .Select(selector)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default) =>
        predicate is null ? Set.CountAsync(cancellationToken) : Set.CountAsync(predicate, cancellationToken);

    public Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        Set.AddAsync(entity, cancellationToken).AsTask();

    public void Remove(T entity) => Set.Remove(entity);
}
