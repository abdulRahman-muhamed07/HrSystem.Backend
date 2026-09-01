namespace HrSystem.Application.Models.Loans;

public sealed record CreateLoanRequest(int EmployeeId, decimal Amount, int Installments, string? Reason);
