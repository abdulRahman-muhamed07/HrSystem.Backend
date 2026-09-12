using HrSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HrSystem.Infrastructure;

public sealed class DesignTimeDbContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Server=localhost;Database=HrSystemDb;Trusted_Connection=True;TrustServerCertificate=True;";
        options.UseSqlServer(connectionString);
        return new AppDbContext(options.Options);
    }
}
