using Erp.Domain.Common;
using Erp.Domain.Exceptions;
using Erp.Domain.Sales.Customers;
using Erp.Domain.Sales.Repositories;
using Erp.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Erp.Application.Sales.Customers.Commands
{
    public sealed record CreateCustomerCommand(
        string Name, string Email, string? Phone, string Street, string City, string? State, string PostalCode, string Country) : IRequest<Guid>;

    public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(c => c.Name).NotEmpty().MaximumLength(150);
            RuleFor(c => c.Email).NotEmpty().MaximumLength(256)
                .EmailAddress().WithMessage("'{PropertyValue}' is not a valid email address.");

            RuleFor(c => c.Phone).MaximumLength(30);
            RuleFor(c => c.Street).NotEmpty().MaximumLength(200);
            RuleFor(c => c.City).NotEmpty().MaximumLength(100);
            RuleFor(c => c.State).MaximumLength(100);
            RuleFor(c => c.PostalCode).NotEmpty().MaximumLength(20);
            RuleFor(c => c.Country).NotEmpty().MaximumLength(100);
        }
    }

    public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
    {
        private readonly ICustomerRepository _customers;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomerCommandHandler(ICustomerRepository customers, IUnitOfWork unitOfWork)
        {
            _customers = customers;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);

            if (await _customers.EmailExistsAsync(email, null, cancellationToken))
                throw new DomainException($"A customer with '{email} already exists.");

            var address = Address.Create(
                request.Street, request.City, request.State, request.PostalCode, request.Country);

            var customer = Customer.Create(request.Name, email, request.Phone, address);

            await _customers.AddAsync(customer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return customer.Id;
        }
    }
}
