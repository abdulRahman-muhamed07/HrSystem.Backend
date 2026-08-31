namespace HrSystem.Application;

public interface ICurrentUser
{
    int? UserId { get; }
    string? UserName { get; }
    string? Role { get; }
}
