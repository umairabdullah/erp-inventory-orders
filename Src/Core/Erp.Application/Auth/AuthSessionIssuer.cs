using Erp.Application.Common.Interfaces;
using Erp.Domain.Identity;

namespace Erp.Application.Auth
{
    public sealed class AuthSessionIssuer
    {
        public readonly ITokenServices _tokens;
        public AuthSessionIssuer(ITokenServices tokens) => _tokens = tokens;

        public AuthResult Issue(User user)
        {
            var rawrefreshTokens = _tokens.GenerateRefreshToken();
            var refreshTokenHash = _tokens.HashRefreshToken(rawrefreshTokens);
            var refreshExpiresAtUtc = DateTime.UtcNow.Add(_tokens.RefreshTokenLifetime);

            user.IssueRefreshToken(refreshTokenHash, refreshExpiresAtUtc);
            var access = _tokens.CreateAccessToken(user);

            return new AuthResult(
                access.Value, access.ExpiresAtUtc, rawrefreshTokens, refreshExpiresAtUtc, new UserDto(user.Id, user.FullName, user.Email.Value, user.Role.ToString()));

        }
    }
}
