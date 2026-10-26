using Erp.Application.Common.Models;

namespace Erp.Application.Inventory.StockMovements
{
    public interface IStockMovementQueries
    {
        Task<PagedResult<StockMovementDto>> GetPagedProductAsync(Guid productId, int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
