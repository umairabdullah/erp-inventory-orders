using Erp.Domain.Identity;

namespace Erp.Application.Common.Interfaces
{
    public sealed record AccessToken(string Value, DateTime ExpiresAtUtc);
    public interface ITokenServices
    {
        TimeSpan RefreshTokenLifetime { get; }
        AccessToken CreateAccessToken(User user);
        string GenerateRefreshToken();
        string HashRefreshToken(string refreshToken);
    }
}
