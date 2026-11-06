using Erp.Application.Common.Interfaces;
using Erp.Application.Sales.Customers;
using Erp.Domain.Sales.Orders;
using System.Globalization;
using System.Net;

namespace Erp.Application.Sales.Orders.Events
{
    public sealed class SendOrderConfirmationEmail : IDomainEventHandler<OrderPlacedEvent>
    {
        private readonly IOrderQueries _orders;
        private readonly ICustomerQueries _customers;
        private readonly IEmailService _email;

        public SendOrderConfirmationEmail(IOrderQueries orders, ICustomerQueries customers, IEmailService email)
        {
            _orders = orders;
            _customers = customers;
            _email = email;
        }

        public async Task Handle(OrderPlacedEvent domainEvent, CancellationToken cancellationToken)
        {
            var order = await _orders.GetByIdAsync(domainEvent.OrderId, cancellationToken);
            var customer = await _customers.GetByIdAsync(domainEvent.CustomerId, cancellationToken);

            if (order is null || customer is null)
                return;

            var rows = string.Join("", order.Items.Select(i => $"""

            <tr>
            <td style="padding:4px 12px 4px 0">{WebUtility.HtmlEncode(i.ProductName)} ({WebUtility.HtmlEncode(i.Sku)})</td>
            <td style="padding:4px 12px; text-align-right">{i.Quantity}</td>
            <td style="padding:4px 12px; text-align-right">{Money(i.UnitPriceAmount)}</td>
            <td style="padding:4px 0 4px 12px; text-align-right">{Money(i.LineTotal)}</td>

            """));

            var notes = string.IsNullOrWhiteSpace(order.Notes) ? "" : $"<p>Notes: {WebUtility.HtmlEncode(order.Notes)}";

            var body = $"""
                <p>Hello {WebUtility.HtmlEncode(customer.Name)},</p>
                <p>Thank you for your order. We have received order <strong>{WebUtility.HtmlEncode(order.OrderNumber)}.</strong></p>
                <table style="border-collapse:collapse">
                <tr>
                    <th style="text-align:left;padding 4px 12px 4px 0">Item</th>
                    <th style="text-aligh:right;padding 4px 12px">Qty</th>
                    <th style="text-aligh:right;padding 4px 12px">Unit Price</th>
                    <th style="text-aligh:right;padding 4px 0 4px 12px">Total</th>
                </tr>
                {rows}
                </table>
                <p><strong>Order total: {Money(order.TotalAmount)} {WebUtility.HtmlEncode(order.TotalCurrency)}</strong></p>
                {notes}
                <p>We will contact you when your order is confirmed.</p>
                """;

            await _email.SendAsync(customer.Email, customer.Name, $"Order {order.OrderNumber} received", body, cancellationToken);

        }
        private static string Money(decimal amount) => amount.ToString("N2", CultureInfo.InvariantCulture);
    }
    
    
} 