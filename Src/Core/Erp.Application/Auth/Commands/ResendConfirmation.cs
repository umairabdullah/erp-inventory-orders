using Erp.Domain.Common;
using Erp.Domain.Identity.Repositories;
using Erp.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Erp.Application.Auth.Commands
{
    public sealed record ResendConfirmationCommand(string Email) : IRequest;
    public sealed class ResendConfirmationCommandValidator : AbstractValidator<ResendConfirmationCommand>
    {
        public ResendConfirmationCommandValidator()
        {
            RuleFor(e => e.Email).NotEmpty().MaximumLength(256).EmailAddress();
        }
    }

    public sealed class ResendConfirmationCommandHandler : IRequestHandler<ResendConfirmationCommand>
    {
        private readonly IUserRepository _users;
        private readonly EmailConfirmationSender _confirmation;
        private readonly IUnitOfWork _unitOfWork;

        public ResendConfirmationCommandHandler(IUserRepository users, EmailConfirmationSender confirmation, IUnitOfWork unitOfWork)
        {
            _users = users;
            _confirmation = confirmation;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ResendConfirmationCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            var user = await _users.GetByEmailAsync(email, cancellationToken);

            if (user is null || user.EmailConfirmed || !user.IsActive)
                return;

            var rawToken = _confirmation.IssueToken(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _confirmation.SendAsync(user, rawToken, cancellationToken);
        }
    }
}
