namespace HrSystem.Application.Abstractions.Security;

public interface IRefreshTokenGenerator
{
    (string RawToken, string TokenHash, DateTime ExpiresAt) Generate();
    string Hash(string rawToken);
}
