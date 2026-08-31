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
        CreateMap<Employee, EmployeeListItem>().ConstructUsing(s =>
            new EmployeeListItem(s.Id, s.FullName, s.Email, s.JobTitle,
                s.Department?.Name ?? string.Empty, s.EmploymentStatus, s.Salary));

        CreateMap<Employee, EmployeeDetails>().ConstructUsing(s =>
            new EmployeeDetails(s.Id, s.Version, s.FullName, s.Email, s.NationalId, s.Phone,
                s.JobTitle, s.DepartmentId, s.Department?.Name ?? string.Empty, s.EmploymentType,
                s.EmploymentStatus, s.Salary, s.HousingAllowance, s.TransportationAllowance,
                s.MealAllowance, s.HireDate));

        CreateMap<Department, DepartmentDto>().ConstructUsing(s =>
            new DepartmentDto(s.Id, s.Name, s.Description, s.Employees.Count));

        CreateMap<AttendanceRecord, AttendanceDto>();

        CreateMap<LeaveRequest, LeaveRequestDto>().ConstructUsing(s =>
            new LeaveRequestDto(s.Id, s.Version, s.EmployeeId, s.Employee?.FullName ?? string.Empty,
                s.LeaveTypeId, s.LeaveType?.Name ?? string.Empty, s.StartDate, s.EndDate,
                s.DurationDays, s.Reason, s.Status, s.RejectionReason));

        CreateMap<EmployeeLoan, LoanDto>().ConstructUsing(s =>
            new LoanDto(s.Id, s.EmployeeId, s.Employee?.FullName ?? string.Empty, s.Amount,
                s.Installments, s.MonthlyDeduction, s.RemainingAmount, s.Reason, s.Status));

        CreateMap<OvertimeRequest, OvertimeDto>().ConstructUsing(s =>
            new OvertimeDto(s.Id, s.EmployeeId, s.Employee?.FullName ?? string.Empty,
                s.Hours, s.RateMultiplier, s.Reason, s.Status));

        CreateMap<PayrollRecord, PayrollDto>().ConstructUsing(s =>
            new PayrollDto(s.Id, s.EmployeeId, s.Employee?.FullName ?? string.Empty,
                s.Year, s.Month, s.BasicSalary, s.HousingAllowance, s.TransportationAllowance,
                s.MealAllowance, s.OtherAllowances, s.OvertimePay, s.GrossSalary,
                s.GosiEmployee, s.GosiEmployer, s.IncomeTax, s.LoanDeduction,
                s.OtherDeductions, s.NetSalary, s.Status));

        CreateMap<User, UserDto>();
        CreateMap<LeaveType, LeaveTypeDto>();

        CreateMap<EmployeeLeaveBalance, LeaveBalanceDto>().ConstructUsing(s =>
            new LeaveBalanceDto(s.Id, s.EmployeeId, s.LeaveTypeId, s.LeaveType?.Name ?? string.Empty,
                s.Year, s.EntitledDays, s.UsedDays, s.AdjustedDays,
                s.EntitledDays + s.AdjustedDays - s.UsedDays));

        CreateMap<AuditLog, AuditLogDto>();
    }
}
