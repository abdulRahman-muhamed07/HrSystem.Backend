using HrSystem.Domain.Enums;

namespace HrSystem.Application.Models.Payroll;

public sealed record PayrollDto(int Id, int EmployeeId, string EmployeeName, int Year, int Month, decimal GrossSalary, decimal NetSalary, decimal OvertimePay, decimal LoanDeduction, PayrollStatus Status, DateTime? PaidAt);
