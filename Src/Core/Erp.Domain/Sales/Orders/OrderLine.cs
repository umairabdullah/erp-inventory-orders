using Erp.Domain.ValueObjects;

namespace Erp.Domain.Sales.Orders
{
    public sealed record OrderLine(Guid ProductId, string ProductName, string Sku, Money UnitPrice, int Quantity);
}
