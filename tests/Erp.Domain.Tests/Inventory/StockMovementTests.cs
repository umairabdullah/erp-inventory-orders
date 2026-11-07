using Erp.Domain.Exceptions;
using Erp.Domain.Inventory;

namespace Erp.Domain.Tests.Inventory
{
    public class StockMovementTests
    {
        private static readonly Guid ProductId = Guid.NewGuid();

        [Fact]
        public void Record_In_save_a_positive_change()
        {
            var moment = StockMovement.RecordIn(ProductId, StockMovementType.In, 5, 15, "Delivery", null);
            Assert.Equal(StockMovementType.In, moment.Type);
            Assert.Equal(5, moment.QuantityChange);
            Assert.Equal(15, moment.BalanceAfter);
        }
        
        [Fact]
        public void Record_out_stores_a_negative_change()
        {
            var moment = StockMovement.RecordOut(ProductId, StockMovementType.Out, 5, 10, "Sold", null);
            Assert.Equal(StockMovementType.Out, moment.Type);
            Assert.Equal(-5, moment.QuantityChange);
        }

        [Fact]
        public void RecordIn_and_recordOut_rejects_zero_quantity()
        {
            Assert.Throws<DomainException>(() => StockMovement.RecordIn(ProductId, StockMovementType.In, 0, 10, null, null));
            Assert.Throws<DomainException>(() => StockMovement.RecordOut(ProductId,StockMovementType.Out,0, 10,null,null));
        }

        [Fact]
        public void Record_adjustment_requires_a_reason()
        {
            Assert.Throws<DomainException>(() => StockMovement.RecordAdjustment(ProductId, -2, 8, " ", null));
        }

        [Fact]
        public void Record_adjustment_rejects_a_zero_change()
        {
            Assert.Throws<DomainException>(() => StockMovement.RecordAdjustment(ProductId, 0, 10, "Recount", null));
        }

        [Fact]
        public void A_movement_cannot_leave_a_negative_balance()
        {
            Assert.Throws<DomainException>(() => StockMovement.RecordOut(ProductId, StockMovementType.Out, 5, -1, null, null));
        }

        [Fact]
        public void A_movement_requires_a_product()
        {
            Assert.Throws<DomainException>(() => StockMovement.RecordIn(Guid.Empty, StockMovementType.In, 5, 5, null, null));
        }
    }
}
