using HrSystem.Domain.Enums;

namespace HrSystem.Application;

public sealed record LoanDto(int Id, int EmployeeId, string EmployeeName, decimal Amount, int Installments, decimal MonthlyDeduction, decimal RemainingAmount, string? Reason, LoanStatus Status);
