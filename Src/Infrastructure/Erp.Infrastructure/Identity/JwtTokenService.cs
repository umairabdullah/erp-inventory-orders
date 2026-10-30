using Erp.Application.Common.Interfaces;
using Erp.Domain.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Erp.Infrastructure.Identity
{
    public sealed class JwtTokenService : ITokenServices
    {
        private readonly JwtSettings _settings;
        private readonly SigningCredentials _credentials;

        public JwtTokenService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;

            if (string.IsNullOrWhiteSpace(_settings.SigningKey) || Encoding.UTF8.GetByteCount(_settings.SigningKey) < 32)
            {
                throw new InvalidOperationException("Jwt: Signing key must be set and atleast 32 bytes long. Use user-secrets in development.");
            }

            _credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SigningKey)), SecurityAlgorithms.HmacSha256); 
        }

        public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(_settings.RefreshTokenDays);

        public AccessToken CreateAccessToken(User user)
        {
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_settings.AccessTokenMinutes);

            var claims = new List<Claim> {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email.Value),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new(ClaimTypes.Role, user.Role.ToString())
                new("role", user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: _credentials
            );

            return new AccessToken(new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

        public string GenerateRefreshToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        public string HashRefreshToken(string refreshToken)
            => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
      
    }
}
