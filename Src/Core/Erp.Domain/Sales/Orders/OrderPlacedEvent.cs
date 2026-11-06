using Erp.Domain.Common;

namespace Erp.Domain.Sales.Orders
{
    public sealed record OrderPlacedEvent(Guid OrderId, string OrderNumber, Guid CustomerId) : IDomainEvent
    {
        public DateTime OccuredAtUtc => DateTime.UtcNow;
    }
}

