using Erp.Domain.Exceptions;
using Erp.Domain.Sales.Orders;
using Erp.Domain.ValueObjects;

namespace Erp.Domain.Tests.Sales
{
    public class OrderTests
    {
        private static OrderLine Line(Guid? ProductId = null, int quantity = 1, decimal price = 10m, string currency = "USD") => new(ProductId ?? Guid.NewGuid(), "Item", "SKU-1", Money.Create(price, currency), quantity);

        private static Order NewOrder(params OrderLine[] lines) =>
            Order.Place("ORD-20261106-0001", Guid.NewGuid(), null, null, lines.Length > 0 ? lines : new[] {Line()} );

        [Fact]
        public void Place_create_a_pending_order_with_a_calculated_total()
        {
            var order = NewOrder(Line(quantity: 2, price: 10m), Line(quantity: 3, price: 5.50m));
            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.Equal(2, order.Items.Count);
            Assert.Equal(36.50m, order.Total.Amount);
            Assert.Equal("USD", order.Total.Currency);
        }

        [Fact]
        public void Place_requires_at_least_one_item()
        {
            Assert.Throws<DomainException>(() => Order.Place("ORD-1", Guid.NewGuid(), null, null, Array.Empty<OrderLine>()));
        }

        [Fact]
        public void Place_requires_a_customer_and_an_order_number()
        {
            Assert.Throws<DomainException>(() => Order.Place("ORD-1", Guid.NewGuid(), null, null, new[] { Line() }));

            Assert.Throws<DomainException>(() => Order.Place("   ", Guid.NewGuid(), null, null, Array.Empty<OrderLine>()));
        }

        [Fact]
        public void Place_rejects_notes_over_500_characters()
        {
            Assert.Throws<DomainException>(() => Order.Place("ORD-1", Guid.NewGuid(), null, new string('x', 501), new[] { Line() }));
        }

        [Fact]
        public void Place_rejects_a_zero_quantity_line()
        {
            Assert.Throws<DomainException>(() => NewOrder(Line(quantity: 0)));
        }

        [Fact]
        public void The_same_product_twice_is_merged_into_one_line()
        {
            var productId = Guid.NewGuid();
            var order = NewOrder(Line(productId, 3, 10m), Line(productId, 2, 10m));

            var item = Assert.Single(order.Items);
            Assert.Equal(3, item.Quantity);
            Assert.Equal(30, order.Total.Amount);
        }

        [Fact]
        public void The_Same_product_with_two_prices_is_rejected()
        {
            var productId = Guid.NewGuid();
            Assert.Throws<DomainException>(() => NewOrder(Line(productId, 1, 10m), Line(productId, 1, 12m)));
        }

        [Fact]
        public void Items_with_different_currencies_are_rejected()
        {
            Assert.Throws<DomainException>(() => NewOrder(Line(price: 10m, currency: "USD"), Line(price: 10m, currency: "EUR")));
        }

        [Fact]
        public void Place_raises_one_OrderPlacedEvent_that_can_be_cleared()
        {
            var order = NewOrder();

            var domainEvent = Assert.IsType<OrderPlacedEvent>(Assert.Single(order.DomainEvents));
            Assert.Equal(order.Id, domainEvent.OrderId);
            Assert.Equal(order.OrderNumber, domainEvent.OrderNumber);

            order.ClearDomainEvents();
            Assert.Empty(order.DomainEvents);
        }

        [Fact]
        public void An_order_moves_through_the_full_Status_flow()
        {
            var order = NewOrder();

            order.Confirm();
            Assert.Equal(OrderStatus.Confirmed, order.Status);

            order.Ship();
            Assert.Equal(OrderStatus.Shipped, order.Status);

            order.Deliver();
            Assert.Equal(OrderStatus.Delivered, order.Status);
        }

        [Fact]
        public void Status_steps_cannot_be_skipped_or_repeated()
        {
            var order = NewOrder();

            Assert.Throws<DomainException>(() => order.Ship());
            Assert.Throws<DomainException>(() => order.Deliver());

            order.Confirm();
            Assert.Throws<DomainException>(() => order.Confirm());
            Assert.Throws<DomainException>(() => order.Deliver());
        }

        [Fact]
        public void A_pending_order_can_be_cancelled_with_a_reason()
        {
            var order = NewOrder();

            order.Cancel("Customer change mind.");

            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.Equal("Customer change mind.", order.CancellationReason);
        }

        [Fact]
        public void A_confirmed_order_can_be_cancelled()
        {
            var order = NewOrder();
            order.Confirm();

            order.Cancel(null);
            Assert.Equal(OrderStatus.Cancelled, order.Status);
        }

        [Fact]
        public void Shipped_and_delivered_orders_cannot_be_cancelled()
        {
            var shipped = NewOrder();
            shipped.Confirm();
            shipped.Ship();
            Assert.Throws<DomainException>(() => shipped.Cancel(null));

            var delivered = NewOrder();
            delivered.Confirm();
            delivered.Ship();
            delivered.Deliver();
            Assert.Throws<DomainException>(() => delivered.Cancel(null));
        }

        [Fact]
        public void A_cancelled_order_cannot_be_cancel_again_or_confirmed()
        {
            var order = NewOrder();
            order.Cancel(null);

            Assert.Throws<DomainException>(() => order.Cancel(null));
            Assert.Throws<DomainException>(() => order.Confirm());
        }

        [Fact]
        public void Cance_rejects_a_reason_over_500_characters()
        {
            Assert.Throws<DomainException>(() => NewOrder().Cancel(new string('x', 501)));
        }
    }
}
