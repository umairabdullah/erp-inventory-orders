using Erp.Application.Common.Models;
using Erp.Domain.Sales.Orders;

namespace Erp.Application.Sales.Orders
{
    public interface IOrderQueries
    {
        Task<PagedResult<OrderSummaryDto>> GetPagedAsync(string? search, OrderStatus? Status, Guid? customerId, int page, int pageSize, CancellationToken cancellationToken);

        Task<OrderDto?> GetByIdAsync (Guid id, CancellationToken cancellationToken = default);
    }
}
