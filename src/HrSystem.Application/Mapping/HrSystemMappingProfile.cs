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
                s.Department == null ? string.Empty : s.Department.Name, s.EmploymentStatus, s.Salary));

        CreateMap<Employee, EmployeeDetails>().ConstructUsing(s =>
            new EmployeeDetails(s.Id, s.Version, s.FullName, s.Email, s.NationalId, s.Phone,
                s.JobTitle, s.DepartmentId, s.Department == null ? string.Empty : s.Department.Name,
                s.EmploymentType, s.EmploymentStatus, s.Salary, s.HousingAllowance,
                s.TransportationAllowance, s.MealAllowance, s.HireDate));

        CreateMap<Department, DepartmentDto>().ConstructUsing(s =>
            new DepartmentDto(s.Id, s.Name, s.Description, s.Employees.Count));

        CreateMap<AttendanceRecord, AttendanceDto>();

        CreateMap<LeaveRequest, LeaveRequestDto>().ConstructUsing(s =>
            new LeaveRequestDto(s.Id, s.EmployeeId,
                s.Employee == null ? string.Empty : s.Employee.FullName,
                s.LeaveTypeId,
                s.LeaveType == null ? string.Empty : s.LeaveType.Name,
                s.StartDate, s.EndDate, s.DurationDays, s.Reason, s.Status, s.RejectionReason));

        CreateMap<EmployeeLoan, LoanDto>().ConstructUsing(s =>
            new LoanDto(s.Id, s.EmployeeId,
                s.Employee == null ? string.Empty : s.Employee.FullName,
                s.Amount, s.Installments, s.MonthlyDeduction, s.RemainingAmount, s.Reason, s.Status));

        CreateMap<OvertimeRequest, OvertimeDto>().ConstructUsing(s =>
            new OvertimeDto(s.Id, s.EmployeeId,
                s.Employee == null ? string.Empty : s.Employee.FullName,
                s.Date, s.Hours, s.RateMultiplier, s.Reason, s.Status));

        CreateMap<PayrollRecord, PayrollDto>().ConstructUsing(s =>
            new PayrollDto(s.Id, s.EmployeeId,
                s.Employee == null ? string.Empty : s.Employee.FullName,
                s.Year, s.Month, s.GrossSalary, s.NetSalary, s.OvertimePay,
                s.LoanDeduction, s.Status, s.PaidAt));

        CreateMap<User, UserDto>();
        CreateMap<LeaveType, LeaveTypeDto>();

        CreateMap<EmployeeLeaveBalance, LeaveBalanceDto>().ConstructUsing(s =>
            new LeaveBalanceDto(s.Id, s.EmployeeId, s.LeaveTypeId,
                s.LeaveType == null ? string.Empty : s.LeaveType.Name,
                s.Year, s.EntitledDays, s.UsedDays, s.AdjustedDays,
                s.EntitledDays + s.AdjustedDays - s.UsedDays));

        CreateMap<AuditLog, AuditLogDto>();
    }
}
