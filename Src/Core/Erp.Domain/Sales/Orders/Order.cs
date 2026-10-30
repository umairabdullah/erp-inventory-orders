using Erp.Domain.Common;
using Erp.Domain.Exceptions;
using Erp.Domain.ValueObjects;

namespace Erp.Domain.Sales.Orders
{
    public sealed class Order : AggregateRoot
    {
        private readonly List<OrderItem> _items = new();

        public string OrderNumber { get; private set; } = string.Empty;
        public Guid CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime PlacedAtUtc { get; private set; }
        public string? Notes { get; private set; }
        public string CancellationReason { get; private set; }
        public Guid? CreatedByUserId { get; private set; }
        public Money Total { get; private set; } = null!;

        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private Order() { }
       
        public static Order Place(string orderNumber, Guid customerId, Guid? createdByUserId, string? notes, IEnumerable<OrderLine> lines)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                throw new DomainException("Order number is required.");

            if (orderNumber.Trim().Length > 30)
                throw new DomainException("Order number cannot exceed 30 characters.");

            if (customerId == Guid.Empty)
                throw new DomainException("Customer is required.");

            var cleanNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();

            if (cleanNotes is { Length: > 500 })
                throw new DomainException("Notes cannot exceed 500 characters.");

            var lineList = lines?.ToList() ?? new List<OrderLine>();

            if(lineList.Count == 0)
                throw new DomainException("An order needs at least one item.");

            var order = new Order
            {
                OrderNumber = orderNumber,
                CustomerId = customerId,
                CreatedByUserId = createdByUserId,
                Notes = cleanNotes,
                Status = OrderStatus.Pending,
                PlacedAtUtc = DateTime.UtcNow,
            };

            foreach (var line in lineList)
                order.AddLine(line);

            return order;

        }

        public void Confirm()
        {
            EnsureStatus(OrderStatus.Pending, "confirmed");
            Status = OrderStatus.Confirmed;
            MarkUpdated();
        }

        public void Ship()
        {
            EnsureStatus(OrderStatus.Confirmed, "shipped");
            Status = OrderStatus.Shipped;
            MarkUpdated();
        }

        public void Deliver()
        {
            EnsureStatus(OrderStatus.Shipped, "delivered");
            Status = OrderStatus.Delivered;
            MarkUpdated();
        }

        public void Cancel(string? reason)
        {
            if (Status is not OrderStatus.Pending or OrderStatus.Confirmed)
                throw new DomainException(
                $"Only Pending or Confirmed orders can be cancelled. This order is {Status}.");

            var cleanReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
            if (cleanReason is { Length: > 500 })
                throw new DomainException("Cancellation reason cannot exceed 500 characters.");

            Status = OrderStatus.Cancelled;
            CancellationReason = cleanReason;
            MarkUpdated();

        }

        private void AddLine(OrderLine line)
        {
            var existing  = _items.FirstOrDefault(i => i.ProductId == line.ProductId);

            if(existing is not null)
            {
                if (existing.UnitPrice != line.UnitPrice)
                    throw new DomainException("The same product cannot appear with two different prices.");

                existing.IncreaseQuantity(line.Quantity);
            }
            else
            {
                if(_items.Count > 0 && _items[0].UnitPrice.Currency != line.UnitPrice.Currency)
                    throw new DomainException("All items in an order must use the same currency.");

                _items.Add(OrderItem.Create(
                    Id, line.ProductId, line.ProductName, line.Sku, line.UnitPrice, line.Quantity));
            }
            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            var currency = _items[0].UnitPrice.Currency;
            Total = _items.Aggregate(Money.Zero(currency), (sum, item) => sum.Add(item.LineTotal));
        }
        private void EnsureStatus(OrderStatus required, string action)
        {
            if (Status != required)
                throw new DomainException($"Only {required} orders can be {action}. This order is {Status}.");

        }
    }
}
