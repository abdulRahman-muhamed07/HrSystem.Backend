using HrSystem.Application;
using HrSystem.Infrastructure.Auditing;
using HrSystem.Infrastructure.Persistence;
using HrSystem.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HrSystem.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=hrsystem.db";
        var provider = configuration["Database:Provider"]?.ToLowerInvariant() ?? "sqlite";

        services.AddDbContext<AppDbContext>(options =>
        {
            if (provider == "sqlserver")
                options.UseSqlServer(connectionString);
            else
                options.UseSqlite(connectionString);
        });

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ILeaveRepository, LeaveRepository>();
        services.AddScoped<IPayrollRepository, PayrollRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IAuditService, AuditService>();

        return services;
    }
}
