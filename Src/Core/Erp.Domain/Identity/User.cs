using Erp.Domain.Common;
using Erp.Domain.Exceptions;
using Erp.Domain.Identity.Repositories;
using Erp.Domain.ValueObjects;
using System.Security.Cryptography;
using System.Text;

namespace Erp.Domain.Identity
{
    public sealed class User : AggregateRoot
    {
        private readonly List<RefreshToken> _refreshTokens = new();

        public string? EmailConfirmationTokenHash { get; private set; }
        public DateTime? EmailConfirmationExpiresAtUtc { get; private set; }
        public string FullName { get; private set; } = string.Empty;
        public Email Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = string.Empty;
        public Role Role {  get; private set; }
        public bool EmailConfirmed {  get; private set; }
        public bool IsActive { get; private set; } = true;


        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

        private User() { } // Ef core

        public static User Register(string fullName, Email email, string passwordHash, Role role)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new DomainException("Full name is required.");

            if (fullName.Trim().Length > 100)
                throw new DomainException("Full name cannot exceed 100 characters.");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Password hash is required.");

            if (!Enum.IsDefined(role))
                throw new DomainException("Unknown role.");

            return new User()
            {
                FullName = fullName.Trim(),
                Email = email,
                PasswordHash = passwordHash,
                Role = role
            };
        }

        public void ConfirmEmail()
        {
            EmailConfirmed = true;
            EmailConfirmationTokenHash = null;
            EmailConfirmationExpiresAtUtc = null;
            MarkUpdated();
        }

        public void ChangeRole(Role role)
        {
            if (!Enum.IsDefined(role))
                throw new DomainException("Unknown role");

            Role = role;
            MarkUpdated();

        }
        public void Activate()
        {
            IsActive = true;
            MarkUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            RevokeAllRefreshTokens();
            MarkUpdated();

        }

        public RefreshToken IssueRefreshToken(string tokenHash, DateTime expiresAtUtc)
        {
            if (!IsActive)
                throw new DomainException("This account is deactivated.");

            var token = RefreshToken.Create(Id, tokenHash, expiresAtUtc);
            _refreshTokens.Add(token);
            return token;
        }
        public RefreshToken? FindRefreshToken(string tokenHash)
          => _refreshTokens.FirstOrDefault(t => t.TokenHash == tokenHash);

        public void RevokeAllRefreshTokens()
        {
            foreach (var token in _refreshTokens.Where(t => !t.IsRevoked))
                token.Revoke();
        }

        public void IssueEmailConfirmationToken(string tokenHash, DateTime expiresAtUtc)
        {
            if (EmailConfirmed)
                throw new DomainException("Email is already confirmed.");

            if (string.IsNullOrWhiteSpace(tokenHash))
                throw new DomainException("Token hash is required.");

            if (expiresAtUtc <= DateTime.UtcNow)
                throw new DomainException("Token expiry must be in the future.");

            EmailConfirmationTokenHash = tokenHash;
            EmailConfirmationExpiresAtUtc = expiresAtUtc;
            MarkUpdated();
        }

        public bool TryConfirmEmail(string tokenHash, DateTime utcNow)
        {
            if (EmailConfirmed || EmailConfirmationTokenHash is null || EmailConfirmationExpiresAtUtc is null)
                return false;

            if(utcNow >= EmailConfirmationExpiresAtUtc)
                return false;

            var stored = Encoding.UTF8.GetBytes(EmailConfirmationTokenHash);
            var given = Encoding.UTF8.GetBytes(tokenHash);

            if (!CryptographicOperations.FixedTimeEquals(stored, given))
                return false;

            ConfirmEmail();
            return true;
        }
     
    }

}
