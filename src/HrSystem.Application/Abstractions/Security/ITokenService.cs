using HrSystem.Domain.Entities;

namespace HrSystem.Application;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) Create(User user);
}
