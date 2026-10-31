using Erp.Application.Common.Exceptions;
using Erp.Domain.Common;
using Erp.Domain.Sales.Repositories;
using FluentValidation;
using MediatR;

namespace Erp.Application.Sales.Customers.Commands
{
    public sealed record SetCustomerStatusCommand(Guid Id, bool IsActive) : IRequest;

    public sealed class SetCustomerStatusCommandValidator : AbstractValidator<SetCustomerStatusCommand>
    {
        public SetCustomerStatusCommandValidator()
        {
            RuleFor(s => s.Id).NotEmpty();
        }
    }

    public sealed class SetCustomerStatusCommandHandler : IRequestHandler<SetCustomerStatusCommand>
    {
        private readonly ICustomerRepository _customers;
        private readonly IUnitOfWork _unitOfWork;

        public SetCustomerStatusCommandHandler(ICustomerRepository customers, IUnitOfWork unitOfWork)
        {
            _customers = customers;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(SetCustomerStatusCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customers.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Customer",  request.Id);

            if (request.IsActive)
                customer.Activate();
            else
                customer.Deactivate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
