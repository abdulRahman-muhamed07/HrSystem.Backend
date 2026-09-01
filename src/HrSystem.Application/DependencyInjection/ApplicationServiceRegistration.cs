using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using HrSystem.Application.Features.Attendance;
using HrSystem.Application.Features.Authentication;
using HrSystem.Application.Features.Auditing;
using HrSystem.Application.Features.Dashboard;
using HrSystem.Application.Features.Departments;
using HrSystem.Application.Features.Employees;
using HrSystem.Application.Features.Employees.Contracts;
using HrSystem.Application.Features.LeaveBalances;
using HrSystem.Application.Features.Leaves;
using HrSystem.Application.Features.Loans;
using HrSystem.Application.Features.Overtime;
using HrSystem.Application.Features.Payroll;
using HrSystem.Application.Features.Users;
using HrSystem.Application.Mapping;
using HrSystem.Application.Services;
using HrSystem.Application.Validators.Authentication;

namespace HrSystem.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(_ => { }, typeof(HrSystemMappingProfile).Assembly);
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

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

        services.AddScoped<AuthenticationHandler>();
        services.AddScoped<EmployeeHandler>();
        services.AddScoped<AttendanceHandler>();
        services.AddScoped<DepartmentHandler>();
        services.AddScoped<LeaveHandler>();
        services.AddScoped<LeaveTypeHandler>();
        services.AddScoped<LeaveBalanceHandler>();
        services.AddScoped<LoanHandler>();
        services.AddScoped<OvertimeHandler>();
        services.AddScoped<PayrollHandler>();
        services.AddScoped<UserHandler>();
        services.AddScoped<DashboardHandler>();
        services.AddScoped<AuditLogHandler>();

        return services;
    }
}
