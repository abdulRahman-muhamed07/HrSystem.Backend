using HrSystem.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HrSystem.Infrastructure.Tests;

[TestClass]
public sealed class InfrastructureTests
{
    [TestMethod]
    public async Task AppDbContext_ShouldCreateDatabaseAndPersistData()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        db.Departments.Add(new HrSystem.Domain.Entities.Department("Engineering"));
        await db.SaveChangesAsync();

        Assert.AreEqual(1, await db.Departments.CountAsync());
    }

    [TestMethod]
    public async Task ExecuteInTransactionAsync_ShouldRollbackOnFailure()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            db.ExecuteInTransactionAsync(async ct =>
            {
                db.Departments.Add(new HrSystem.Domain.Entities.Department("Temporary"));
                await db.SaveChangesAsync(ct);
                throw new InvalidOperationException("force rollback");
                #pragma warning disable CS0162
                return true;
                #pragma warning restore CS0162
            }, CancellationToken.None));

        Assert.AreEqual(0, await db.Departments.CountAsync());
    }
}
