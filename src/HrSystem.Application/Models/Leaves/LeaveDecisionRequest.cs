namespace HrSystem.Application;

public sealed record LeaveDecisionRequest(bool Approve, string? RejectionReason);
