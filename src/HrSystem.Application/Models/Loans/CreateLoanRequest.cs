namespace HrSystem.Application;

public sealed record CreateLoanRequest(int EmployeeId, decimal Amount, int Installments, string? Reason);
