using Erp.Application.Common.Interfaces;
using Erp.Domain.Identity;
using System.Net;

namespace Erp.Application.Auth
{
    public sealed class EmailConfirmationSender
    {
        public static readonly TimeSpan TokenLifeTime = TimeSpan.FromHours(24);

        private readonly ITokenServices _tokens;
        private readonly IEmailService _email;
        private readonly IAppLinks _links;

        public EmailConfirmationSender(ITokenServices tokens, IEmailService email, IAppLinks links)
        {
            _tokens = tokens;
            _email = email;
            _links = links;
        }

        public string IssueToken(User user)
        {
            var rawToken = _tokens.GenerateUrlSafeToken();
            user.IssueEmailConfirmationToken(_tokens.HashToken(rawToken), DateTime.UtcNow.Add(TokenLifeTime));
            return rawToken;
        }
        
        public Task<bool> SendAsync(User user , string rawToken, CancellationToken cancellationToken)
        {
            var link = _links.BuildEmailConfirmationLink(user.Email.Value, rawToken);
            var body = $"""
                <p>Hello {WebUtility.HtmlEncode(user.FullName)},</p>
                <p>Please confirm your email address to activate your account.</p>
                <p><a href="{WebUtility.HtmlEncode(link)}">Confirm My Email</a></p>
                <p>This link expires in 24 hours. If you did not create this account, you can ignore this email.</p>
                """;
            return _email.SendAsync(user.Email.Value, user.FullName, "Confirm your email address", body, cancellationToken);
        }
    }
}
