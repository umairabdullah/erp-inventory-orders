using Erp.Domain.Sales.Orders;

namespace Erp.Application.Sales.Orders
{
    public sealed record OrderSummaryDto(Guid Id, string OrderNumber, Guid CustomerId, string CustomerName, OrderStatus Status, DateTime PlaceAtUtc, decimal TotalAmount, string TotalCurrency, int ItemCount);

    public sealed record OrderItemDto(Guid ProductId, string ProductName, string Sku, decimal UnitPriceAmount, int Quantity, decimal LineTotal);

    public sealed record OrderDto(
         Guid Id, string OrderNumber, Guid CustomerId, string CustomerName,
         OrderStatus Status, DateTime PlacedAtUtc, string? Notes, string? CancellationReason,
         Guid? CreatedByUserId, decimal TotalAmount, string TotalCurrency, IReadOnlyList<OrderItemDto> Items
    );
}
