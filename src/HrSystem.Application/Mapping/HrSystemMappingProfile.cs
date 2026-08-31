using AutoMapper;
using HrSystem.Application.Models.Attendance;
using HrSystem.Application.Models.Auditing;
using HrSystem.Application.Models.Departments;
using HrSystem.Application.Models.Employees;
using HrSystem.Application.Models.Leaves;
using HrSystem.Application.Models.Loans;
using HrSystem.Application.Models.Overtime;
using HrSystem.Application.Models.Payroll;
using HrSystem.Application.Models.Users;
using HrSystem.Domain.Entities;

namespace HrSystem.Application.Mapping;

public sealed class HrSystemMappingProfile : Profile
{
    public HrSystemMappingProfile()
    {
        CreateMap<Employee, EmployeeListItem>()
            .ForCtorParam("DepartmentName", o => o.MapFrom(s => s.Department != null ? s.Department.Name : string.Empty));

        CreateMap<Employee, EmployeeDetails>()
            .ForCtorParam("DepartmentName", o => o.MapFrom(s => s.Department != null ? s.Department.Name : string.Empty));

        CreateMap<Department, DepartmentDto>()
            .ForCtorParam("EmployeeCount", o => o.MapFrom(s => s.Employees.Count));

        CreateMap<AttendanceRecord, AttendanceDto>();

        CreateMap<LeaveRequest, LeaveRequestDto>()
            .ForCtorParam("EmployeeName", o => o.MapFrom(s => s.Employee != null ? s.Employee.FullName : string.Empty))
            .ForCtorParam("LeaveTypeName", o => o.MapFrom(s => s.LeaveType != null ? s.LeaveType.Name : string.Empty));

        CreateMap<EmployeeLoan, LoanDto>()
            .ForCtorParam("EmployeeName", o => o.MapFrom(s => s.Employee != null ? s.Employee.FullName : string.Empty));

        CreateMap<OvertimeRequest, OvertimeDto>()
            .ForCtorParam("EmployeeName", o => o.MapFrom(s => s.Employee != null ? s.Employee.FullName : string.Empty));

        CreateMap<PayrollRecord, PayrollDto>()
            .ForCtorParam("EmployeeName", o => o.MapFrom(s => s.Employee != null ? s.Employee.FullName : string.Empty));

        CreateMap<User, UserDto>();
        CreateMap<LeaveType, LeaveTypeDto>();

        CreateMap<EmployeeLeaveBalance, LeaveBalanceDto>()
            .ForCtorParam("LeaveTypeName", o => o.MapFrom(s => s.LeaveType != null ? s.LeaveType.Name : string.Empty))
            .ForCtorParam("AvailableDays", o => o.MapFrom(s => s.EntitledDays + s.AdjustedDays - s.UsedDays));

        CreateMap<AuditLog, AuditLogDto>();
    }
}
