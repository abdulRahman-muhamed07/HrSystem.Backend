namespace HrSystem.Application.Models.Leaves;

public sealed record CreateLeaveTypeRequest(
    string Name,
    decimal DaysPerYear,
    bool IsPaid,
    string? NameAr = null,
    string? Description = null);
