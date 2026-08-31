using HrSystem.Domain.Entities;
using HrSystem.Infrastructure.Persistence;
using HrSystem.Infrastructure.Security;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HrSystem.Infrastructure.Tests;

[TestClass]
public sealed class SecurityAndConcurrencyTests
{
    [TestMethod]
    public void RefreshTokenGenerator_ShouldCreateUniqueTokensAndHashes()
    {
        var configuration = new ConfigurationManager();
        configuration["Jwt:RefreshTokenExpirationDays"] = "7";
        var generator = new RefreshTokenGenerator(configuration);
        var first = generator.Generate();
        var second = generator.Generate();
        Assert.AreNotEqual(first.RawToken, second.RawToken);
        Assert.AreEqual(first.TokenHash, generator.Hash(first.RawToken));
        Assert.IsTrue(first.ExpiresAt > DateTime.UtcNow);
    }

    [TestMethod]
    public async Task Employee_ShouldUseOptimisticConcurrency()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;

        await using var setup = new AppDbContext(options);
        await setup.Database.EnsureCreatedAsync();
        var department = new Department("Engineering");
        setup.Departments.Add(department);
        await setup.SaveChangesAsync();
        setup.Employees.Add(new Employee("Original", "original@example.com", "Developer", department.Id, 1000m, DateTime.UtcNow));
        await setup.SaveChangesAsync();

        await using var first = new AppDbContext(options);
        await using var second = new AppDbContext(options);
        var employee1 = await first.Employees.SingleAsync();
        var employee2 = await second.Employees.SingleAsync();
        employee1.UpdateProfile(employee1.FullName, employee1.Email, "Senior Developer", employee1.DepartmentId, employee1.Salary, employee1.EmploymentType, employee1.EmploymentStatus, employee1.Phone, employee1.Address);
        await first.SaveChangesAsync();
        employee2.UpdateProfile(employee2.FullName, employee2.Email, "Lead Developer", employee2.DepartmentId, employee2.Salary, employee2.EmploymentType, employee2.EmploymentStatus, employee2.Phone, employee2.Address);
        await Assert.ThrowsExceptionAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync());
    }

    [TestMethod]
    public async Task UnitOfWork_ShouldRollbackTransactionOnFailure()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
            await db.ExecuteInTransactionAsync(async ct =>
            {
                db.Departments.Add(new Department("Temporary"));
                await db.SaveChangesAsync(ct);
                throw new InvalidOperationException("rollback");
            }));

        Assert.AreEqual(0, await db.Departments.CountAsync());
    }
}
