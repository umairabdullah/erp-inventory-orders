using Erp.Application.Common.Interfaces;
using Erp.Domain.Common;
using Erp.Domain.Identity.Repositories;
using FluentValidation;
using MediatR;

namespace Erp.Application.Auth.Commands
{
    public sealed record LogoutCommand(string RefreshToken) : IRequest;
    public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }

    public sealed class LogoutCommandValidatorHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IUserRepository _users;
        private readonly ITokenServices _tokens;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandValidatorHandler(IUserRepository users, ITokenServices tokens, IUnitOfWork unitOfWork)
        {
            _users = users;
            _tokens = tokens;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var hash = _tokens.HashRefreshToken(request.RefreshToken);
            var user = await _users.GetByRefreshTokenHashAsync(hash, cancellationToken);

            if (user is null)
                return;

            user.FindRefreshToken(hash)?.Revoke();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
