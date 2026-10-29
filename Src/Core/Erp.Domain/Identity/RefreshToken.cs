using Erp.Domain.Common;
using Erp.Domain.Exceptions;

namespace Erp.Domain.Identity
{
    public sealed class RefreshToken : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string TokenHash { get; private set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; private set; }
        public DateTime? RevokedAtUtc { get; private set; }
        public string? ReplacedByTokenHash { get; private set; }
        public bool IsRevoked => RevokedAtUtc.HasValue;

        private RefreshToken() { } // Ef Core

        internal static RefreshToken Create(Guid userId, string tokenHash, DateTime expiresAtUtc)
        {
            if (userId == Guid.Empty)
                throw new DomainException("User is required");

            if (string.IsNullOrWhiteSpace(tokenHash))
                throw new DomainException("Token hash is required");

            if (expiresAtUtc <= DateTime.UtcNow)
                throw new DomainException("Token expiry must be in future.");

            return new RefreshToken()
            {
                UserId = userId,
                TokenHash = tokenHash,
                ExpiresAtUtc = expiresAtUtc
            };
        }

        public bool IsExpired(DateTime utcNow) => utcNow >= ExpiresAtUtc;
        public bool IsActive(DateTime utcNow) => !IsRevoked && !IsExpired(utcNow);

        public void Revoke(string? replacedByTokenHash = null)
        {
            if (IsRevoked)
                return;

            RevokedAtUtc = DateTime.UtcNow;
            ReplacedByTokenHash = replacedByTokenHash;
        }

    }
}
