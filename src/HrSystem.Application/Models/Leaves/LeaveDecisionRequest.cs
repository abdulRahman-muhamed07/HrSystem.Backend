namespace HrSystem.Application.Models.Leaves;

public sealed record LeaveDecisionRequest(bool Approve, string? RejectionReason);
