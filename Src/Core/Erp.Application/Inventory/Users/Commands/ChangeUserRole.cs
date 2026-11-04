using Erp.Application.Common.Exceptions;
using Erp.Application.Common.Interfaces;
using Erp.Domain.Common;
using Erp.Domain.Identity.Repositories;
using FluentValidation;
using MediatR;

namespace Erp.Application.Inventory.Users.Commands
{
    public sealed record  ChangeUserRoleCommand(Guid Id, Role Role): IRequest;
    public sealed class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
    {
        public ChangeUserRoleCommandValidator() 
        {
            RuleFor(u => u.Id).NotEmpty();
            RuleFor(u => u.Role).IsInEnum();
        }
    }

    public sealed class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand>
    {
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public ChangeUserRoleCommandHandler(IUserRepository users, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _users = users;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == _currentUser.UserId)
                throw new ForbiddenException("You cannot change your own role.");

            var user = await _users.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("User", request.Id);

            user.ChangeRole(request.Role);

            user.RevokeAllRefreshTokens();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
