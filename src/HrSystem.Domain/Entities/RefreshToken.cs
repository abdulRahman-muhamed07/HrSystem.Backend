namespace HrSystem.Domain.Entities;

public sealed class RefreshToken
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }
    public User? User { get; private set; }

    private RefreshToken() { }

    public RefreshToken(int userId, string tokenHash, DateTime expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
    }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;

    public void Revoke(string? replacedByTokenHash = null)
    {
        RevokedAt ??= DateTime.UtcNow;
        ReplacedByTokenHash = replacedByTokenHash;
    }
}
