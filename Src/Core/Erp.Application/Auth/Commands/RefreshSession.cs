using Erp.Application.Common.Exceptions;
using Erp.Application.Common.Interfaces;
using Erp.Domain.Common;
using Erp.Domain.Identity.Repositories;
using FluentValidation;
using MediatR;

namespace Erp.Application.Auth.Commands
{
    public sealed record RefreshSessionCommand(string RefreshToken) : IRequest<AuthResult>;
    public sealed class RefreshSessionValidator : AbstractValidator<RefreshSessionCommand>
    {
        public RefreshSessionValidator() 
        {
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }

    public sealed class RefreshSessionCommandHandler : IRequestHandler<RefreshSessionCommand, AuthResult>
    {
        private const string InvalidMessage = "Invalid refresh token.";

        private readonly IUserRepository _users;
        private readonly ITokenServices _tokens;
        private readonly AuthSessionIssuer _issuer;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshSessionCommandHandler(IUserRepository users, ITokenServices tokens, AuthSessionIssuer issuer, IUnitOfWork unitOfWork)
        {
            _users = users;
            _tokens = tokens;
            _issuer = issuer;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResult> Handle(RefreshSessionCommand request, CancellationToken cancellationToken)
        {
            var hash = _tokens.HashRefreshToken(request.RefreshToken);
            var user = await _users.GetByRefreshTokenHashAsync(hash, cancellationToken) ?? throw new UnauthorizedException(InvalidMessage);

            var stored = user.FindRefreshToken(hash) ?? throw new UnauthorizedException(InvalidMessage);

            if (stored.IsRevoked)
            {
                user.RevokeAllRefreshTokens();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                throw new UnauthorizedException("This refresh token was already used.Please sign in again.");
            }

            if (!user.IsActive || stored.IsExpired(DateTime.UtcNow)) throw new UnauthorizedException(InvalidMessage);

            var result = _issuer.Issue(user);
            stored.Revoke(_tokens.HashRefreshToken(result.RefreshToken));

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result;
        }
    }
}
