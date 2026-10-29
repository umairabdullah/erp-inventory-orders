using Erp.Application.Common.Exceptions;
using Erp.Application.Common.Interfaces;
using Erp.Domain.Common;
using Erp.Domain.Identity;
using Erp.Domain.Identity.Repositories;
using Erp.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Erp.Application.Auth.Commands
{
    public sealed record RegisterCommand(string FullName, string Email , string Password ) : IRequest<Guid>;

    public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("'{PropertyValue}' is not a valid email address.")
            .MaximumLength(256);

            RuleFor(x => x.Password)
                .NotEmpty().MinimumLength(8).MaximumLength(100)
                .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain a lowerase letter.")
                .Matches("[0-9]").WithMessage("Password must contain a digit.");
        }
    }

    public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
    {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IUnitOfWork _unitOfWork;
        
        public RegisterCommandHandler(IUserRepository users, IPasswordHasher hasher, IUnitOfWork unitOfWork)
        {
            _users = users;
            _hasher = hasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            if (await _users.EmailExistsAsync(email, cancellationToken))
                throw new ConflictException("An account with this email is already exists.");

            var passwordHash = _hasher.Hash(request.Password);
            var user = User.Register(request.FullName, email, passwordHash, Role.Staff);

            await _users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}
