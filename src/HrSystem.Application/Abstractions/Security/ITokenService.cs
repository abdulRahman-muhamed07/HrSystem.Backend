using HrSystem.Domain.Entities;

namespace HrSystem.Application.Abstractions.Security;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) Create(User user);
}
