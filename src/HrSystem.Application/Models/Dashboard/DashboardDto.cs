namespace HrSystem.Application;

public sealed record DashboardDto(int EmployeeCount, int ActiveEmployeeCount, int PendingLeaves, int PendingOvertime, int PendingLoans, decimal PayrollNetThisMonth);
