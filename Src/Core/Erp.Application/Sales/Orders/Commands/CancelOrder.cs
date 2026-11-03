using Erp.Application.Common.Exceptions;
using Erp.Application.Common.Interfaces;
using Erp.Domain.Common;
using Erp.Domain.Inventory;
using Erp.Domain.Inventory.Repositories;
using Erp.Domain.Sales.Orders;
using Erp.Domain.Sales.Repositories;
using FluentValidation;
using MediatR;

namespace Erp.Application.Sales.Orders.Commands
{
    public sealed record CancelOrderCommand(Guid Id, string? Reason) : IRequest;

    public sealed class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator()
        {
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.Reason).MaximumLength(500);
        }
    }

    public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
    {
        private readonly IOrderRepository _orders;
        private readonly IProductRepository _products;
        private readonly IStockMovementRepository _movements;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public CancelOrderCommandHandler(IOrderRepository orders, IProductRepository products, IStockMovementRepository movements, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _orders = orders;
            _products = products;
            _movements = movements;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orders.GetByIdAsync(request.Id, cancellationToken) ??
                throw new NotFoundException("Order", request.Id);

            if (!_currentUser.IsManagerOrAdmin)
            {
                var isOwner = order.CreatedByUserId.HasValue && order.CreatedByUserId == _currentUser.UserId;

                if (!isOwner || order.Status != OrderStatus.Pending)
                    throw new ForbiddenException("Staff can only cancel their own Pending orders.");
            }
            order.Cancel(request.Reason);

            var quantities = order.Items.ToDictionary(i => i.ProductId, i => i.Quantity);
            var products = await _products.GetByIdAsync(quantities.Keys.ToList(), cancellationToken);

            foreach(var product in products)
            {
                var quantity = quantities[product.Id];
                product.AddStock(quantity);

                var movement = StockMovement.RecordIn(product.Id, StockMovementType.In, quantity, product.QuantityInStock, $"Order {order.OrderNumber} Cancelled", _currentUser.UserId);

                await _movements.AddAsync(movement, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
