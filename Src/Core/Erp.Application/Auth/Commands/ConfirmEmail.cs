using Erp.Application.Common.Interfaces;
using Erp.Domain.Common;
using Erp.Domain.Exceptions;
using Erp.Domain.Identity.Repositories;
using Erp.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Erp.Application.Auth.Commands
{
    public sealed record ConfirmEmailCommand(string Email, string Token) : IRequest;

    public sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(e => e.Email).NotEmpty().MaximumLength(256).EmailAddress();
            RuleFor(e => e.Token).NotEmpty().MaximumLength(200);
        }
    }

    public sealed class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand>
    {
        private readonly IUserRepository _users;
        private readonly ITokenServices _tokens;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmEmailCommandHandler(IUserRepository users, ITokenServices tokens, IUnitOfWork unitOfWork)
        {
            _users = users;
            _tokens = tokens;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            var user = await _users.GetByEmailAsync(email, cancellationToken);

            if (user is null || !user.TryConfirmEmail(_tokens.HashToken(request.Token), DateTime.UtcNow))
                throw new DomainException("This confirmation link is invalid or has expired.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
