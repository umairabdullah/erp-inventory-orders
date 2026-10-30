using Erp.Domain.Common;
using Erp.Domain.Exceptions;
using Erp.Domain.ValueObjects;

namespace Erp.Domain.Sales.Orders
{
    public sealed class OrderItem : BaseEntity
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; } = string.Empty;
        public string ProductSku { get; private set; } = string.Empty;
        public Money UnitPrice { get; private set; } = null!;
        public int Quantity { get; private set; }

        public Money LineTotal => UnitPrice.Multiply(Quantity);
        private OrderItem() { }

        internal static OrderItem Create(Guid orderId, Guid productId, string productName, string productSku, Money unitPrice, int quantity)
        {
            if (orderId == Guid.Empty)
                throw new DomainException("Order is required.");

            if (productId == Guid.Empty)
                throw new DomainException("Product is required.");

            if (string.IsNullOrWhiteSpace(productName))
                throw new DomainException("Product name is required.");

            if (string.IsNullOrWhiteSpace(productSku))
                throw new DomainException("Product SKU is required.");

            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            return new OrderItem
            {
                OrderId = orderId,
                ProductId = productId,
                ProductName = productName.Trim(),
                ProductSku = productSku.Trim(),
                UnitPrice = unitPrice ?? throw new DomainException("Unit price is required."),
                Quantity = quantity
            };

        }

        internal void IncreaseQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            quantity += quantity;
            MarkUpdated();
        }

    }
}
