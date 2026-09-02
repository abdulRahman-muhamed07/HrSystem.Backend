namespace HrSystem.Application.Abstractions.Security;

public interface ICurrentUser
{
    int? UserId { get; }
    string? UserName { get; }
    string? Role { get; }
}
