using Erp.Domain.Common;
using Erp.Domain.Exceptions;

namespace Erp.Domain.Inventory
{
    public sealed class StockMovement : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public StockMovementType Type { get; private set; }
        public int QuantityChange { get; private set; }
        public int BalanceAfter { get; private set; }
        public string? Reason { get; private set; }
        public Guid? PerformedByUserId { get; private set; }

        private StockMovement() { }
        


        public static StockMovement RecordIn(Guid productId, StockMovementType type, int quantity, int balanceAfter, string? reason, Guid? performedByUserId)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity received must be greater than zero.");

            return Build(productId, type, quantity, balanceAfter, reason, performedByUserId);
        }

        public static StockMovement RecordOut(Guid productId, StockMovementType type, int quantity, int balanceAfter, string? reason, Guid? performedByUserId)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity received must be greater than zero.");

            return Build(productId, StockMovementType.Out, -quantity, balanceAfter, reason, performedByUserId);
        }

        public static StockMovement RecordAdjustment(Guid productId, int quantityChange, int balanceAfter, string? reason, Guid? performedByUserId)
        {
            if (quantityChange == 0)
                throw new DomainException("An adjustment must change the quantity.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("A reason is required for stock adjustment.");

            return Build(productId, StockMovementType.Adjustment, quantityChange, balanceAfter, reason, performedByUserId);
        }

        public static StockMovement Build(Guid productId, StockMovementType type, int quantityChange, int balanceAfter, string? reason, Guid? performedByUserId)
        {
            if (productId == Guid.Empty)
                throw new DomainException("Product is required.");
            if (balanceAfter < 0)
                throw new DomainException("Stock balance cannot be negative.");
            var cleanReason = reason?.Trim();

            if (cleanReason is { Length: > 500 })
                throw new DomainException("Reason cannot exceed 500 characters.");

            return new StockMovement
            {
                ProductId = productId,
                Type = type,
                QuantityChange = quantityChange,
                BalanceAfter = balanceAfter,
                Reason = cleanReason,
                PerformedByUserId = performedByUserId
            };
        }

        
    }
}
