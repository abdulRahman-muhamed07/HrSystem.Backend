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
}
