using Erp.Application.Common.Exceptions;
using Erp.Domain.Common;
using Erp.Domain.Sales.Customers;
using Erp.Domain.Sales.Repositories;
using Erp.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Erp.Application.Sales.Customers.Commands
{
    public sealed record UpdateCustomerCommand(Guid Id, string Name, string Email, string? Phone, string Street, string City, string? State, string PostalCode, string Country): IRequest;


    public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.Name).NotEmpty().MaximumLength(150);
            RuleFor(c => c.Email).NotEmpty().MaximumLength(256).EmailAddress().WithMessage("'{PropertyValue}' is not a valid email address.");
            RuleFor(c => c.Phone).MaximumLength(30);
            RuleFor(c => c.Street).NotEmpty().MaximumLength(200);
            RuleFor(c => c.City).NotEmpty().MaximumLength(100);
            RuleFor(c => c.State).MaximumLength(100);
            RuleFor(c => c.PostalCode).NotEmpty().MaximumLength(20);
            RuleFor(c => c.Country).NotEmpty().MaximumLength(100);
        }
    }

    public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
    {
        private readonly ICustomerRepository _customers;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCustomerCommandHandler(ICustomerRepository customers, IUnitOfWork unitOfWork)
        {
            _customers = customers;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customers.GetByIdAsync(request.Id, cancellationToken) ??
                throw new NotFoundException("Customer", request.Id);

            var email = Email.Create(request.Email);
            if (await _customers.EmailExistsAsync(email, customer.Id, cancellationToken))
                throw new ConflictException($"Another customer already uses the email '{email}'.");

            var address = Address.Create(request.Street,request.City,request.State, request.PostalCode, request.Country);

            customer.Update(request.Name, email, request.Phone, address);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
