namespace HrSystem.Application;

public interface ICurrentUser
{
    int? UserId { get; }
    int? EmployeeId { get; }
    string? UserName { get; }
    string? Role { get; }
}
