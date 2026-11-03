using Erp.Application.Common.Exceptions;
using Erp.Domain.Common;
using Erp.Domain.Exceptions;
using Erp.Domain.Sales.Orders;
using Erp.Domain.Sales.Repositories;
using FluentValidation;
using MediatR;

namespace Erp.Application.Sales.Orders.Commands
{
    public sealed record ChangeOrderStatusCommand(Guid Id, OrderStatus Status) : IRequest;
    
    public sealed class ChangeOrderStatusCommandValidator : AbstractValidator<ChangeOrderStatusCommand>
    {

        public ChangeOrderStatusCommandValidator()
        {
            RuleFor(y => y.Id).NotEmpty();
            RuleFor(y => y.Status)
                .NotEqual(OrderStatus.Cancelled)
                 .WithMessage("Use the cancel endpoint to cancel the order.");

            RuleFor(y => y.Status)
                .Must(t => t is OrderStatus.Confirmed or OrderStatus.Shipped or OrderStatus.Delivered)
                .WithMessage("Unsupported status changed.");
        }
    }

    public sealed class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand>
    {
        private readonly IOrderRepository _orders;
        private readonly IUnitOfWork _unitOfWork;

        public ChangeOrderStatusCommandHandler(IOrderRepository orders, IUnitOfWork unitOfWork)
        {
            _orders = orders;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _orders.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Order", request.Id);

            switch (request.Status)
            {
                case OrderStatus.Confirmed:
                    order.Confirm();
                    break;
                case OrderStatus.Shipped:
                    order.Ship();
                    break;
                case OrderStatus.Delivered:
                    order.Deliver();
                    break;
             
                default:
                    throw new DomainException("Unsupported status changed.");
                    
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
