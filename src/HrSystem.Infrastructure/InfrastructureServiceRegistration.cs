using HrSystem.Application;
using HrSystem.Infrastructure.Auditing;
using HrSystem.Infrastructure.Persistence;
using HrSystem.Infrastructure.Persistence.Repositories;
using HrSystem.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HrSystem.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required. Provide it through User Secrets in development or environment/secret configuration in production.");

        services.AddMemoryCache();
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<ILeaveRepository, LeaveRepository>();
        services.AddScoped<IPayrollRepository, PayrollRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddSingleton<ITokenRevocationService, InMemoryTokenRevocationService>();
        services.AddScoped<IAuditService, AuditService>();

        return services;
    }
}
