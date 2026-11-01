using Erp.Application.Common.Exceptions;
using Erp.Application.Common.Interfaces;
using Erp.Domain.Common;
using Erp.Domain.Exceptions;
using Erp.Domain.Inventory;
using Erp.Domain.Inventory.Repositories;
using Erp.Domain.Sales.Orders;
using Erp.Domain.Sales.Repositories;
using FluentValidation;
using MediatR;

namespace Erp.Application.Sales.Orders.Commands
{
    public sealed record PlaceOrderItem(Guid ProductId, int Quantity);

    public sealed record PlaceOrderCommand (Guid CustomerId, string? Notes, List<PlaceOrderItem> Items) : IRequest<PlaceOrderResult>;

    public sealed record PlaceOrderResult(Guid Id, string OrderNumber);

    public sealed class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
    {
        public PlaceOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.Notes).MaximumLength(500);

            RuleFor(x => x.Items).NotEmpty().WithMessage("An order needs at least one item.");
            RuleFor(x => x.Items)
                .Must(items => items is null || items.Count <= 100)
                .WithMessage("An order can have at most 100 lines.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty();
                item.RuleFor(i => i.Quantity).InclusiveBetween(1, 1000);

            });

        }
    }

    public sealed class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, PlaceOrderResult>
    {
        private readonly ICustomerRepository _customers;
        private readonly IProductRepository _products;
        private readonly IOrderRepository _orders;
        private readonly IStockMovementRepository _movements;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public PlaceOrderCommandHandler(ICustomerRepository customers, IProductRepository products, IOrderRepository orders, IStockMovementRepository movements, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            _customers = customers;
            _products = products;
            _orders = orders;
            _movements = movements;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<PlaceOrderResult> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customers.GetByIdAsync(request.CustomerId, cancellationToken)   ?? throw new NotFoundException("Customer", request.CustomerId);

            if (!customer.IsActive)
                throw new DomainException("This customer is inactive and cannot place orders.");

            var quantities = request.Items
                .GroupBy(i => i.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));

            var products = await _products.GetByIdAsync(quantities.Keys.ToList(), cancellationToken);

            foreach(var productId in quantities.Keys)
            {
                if (products.All(p => p.Id != productId))
                    throw new NotFoundException("Product", productId);
            }

            var lines = new List<OrderLine>();

            foreach(var product in products.OrderBy(x => x.Name))
            {
                var quantity = quantities[product.Id];

                if (!product.IsActive)
                    throw new DomainException($"Product '{product.Name}' is inactive and cannot be ordered.");

                if (product.QuantityInStock < quantity)
                    throw new DomainException($"Insufficent stock for '{product.Name}'. Available '{product.QuantityInStock}', requested: '{quantity}'.");

                lines.Add(new OrderLine(product.Id, product.Name, product.Sku.Value, product.Price, quantity));
            }

            var prefix = $"ORD-{DateTime.UtcNow:yyyyMMdd}-";

            //here the error will generate

            var sequence = await _orders.CountByNumberPrefixAsync(prefix, cancellationToken) + 1;

            var orderNumber = $"{prefix}{sequence:D4}";

            var order = Order.Place(orderNumber, customer.Id, _currentUser.UserId, request.Notes, lines);

            foreach(var product in products)
            {
                var quantity = quantities[product.Id];
                product.RemoveStock(quantity);


                //and also here the error will generate

                var movement = StockMovement.RecordOut(product.Id, StockMovementType.Out, quantity, product.QuantityInStock, $"Order {order.OrderNumber}", _currentUser.UserId);

                await _movements.AddAsync(movement, cancellationToken);
            }

            await _orders.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PlaceOrderResult(order.Id, order.OrderNumber);
        }
    }
}
