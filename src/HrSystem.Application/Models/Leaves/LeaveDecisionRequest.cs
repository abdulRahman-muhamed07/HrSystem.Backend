namespace HrSystem.Application.Models.Leaves;

public sealed record LeaveDecisionRequest(Guid Version, bool Approve, string? RejectionReason);
