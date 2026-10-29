using Erp.Application.Common.Exceptions;
using Erp.Application.Common.Interfaces;
using Erp.Domain.Common;
using Erp.Domain.Identity.Repositories;
using Erp.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Erp.Application.Auth.Commands
{
    public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResult>;

    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(l => l.Email).NotEmpty().EmailAddress();
            RuleFor(l => l.Password).NotEmpty();
        }
    }

    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly IUserRepository _user;
        private readonly IPasswordHasher _hasher;
        private readonly AuthSessionIssuer _issuer;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(IUserRepository user, IPasswordHasher hasher, AuthSessionIssuer issuer, IUnitOfWork unitOfWork)
        {
            _user = user;
            _hasher = hasher;
            _issuer = issuer;
            this._unitOfWork = unitOfWork;
        }

        public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            var user = await _user.GetByEmailAsync(email, cancellationToken);

            if (user is null || !user.IsActive || !_hasher.Verify(user.PasswordHash, request.Password))
                throw new UnauthorizedException("Invalid email or password");

            var result = _issuer.Issue(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}