using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using HrSystem.Application.Mapping;
using HrSystem.Application.Services;
using HrSystem.Application.Validators.Authentication;

namespace HrSystem.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(_ => { }, typeof(HrSystemMappingProfile).Assembly);
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>(ServiceLifetime.Scoped);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<ILeaveService, LeaveService>();
        services.AddScoped<IOvertimeService, OvertimeService>();
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<IPayrollService, PayrollService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILeaveTypeService, LeaveTypeService>();
        services.AddScoped<ILeaveBalanceReadService, LeaveBalanceReadService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        return services;
    }
}
