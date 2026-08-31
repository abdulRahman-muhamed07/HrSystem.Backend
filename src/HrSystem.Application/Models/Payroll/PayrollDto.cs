using HrSystem.Domain.Enums;

namespace HrSystem.Application;

public sealed record PayrollDto(int Id, int EmployeeId, string EmployeeName, int Year, int Month, decimal GrossSalary, decimal NetSalary, decimal OvertimePay, decimal LoanDeduction, PayrollStatus Status, DateTime? PaidAt);
