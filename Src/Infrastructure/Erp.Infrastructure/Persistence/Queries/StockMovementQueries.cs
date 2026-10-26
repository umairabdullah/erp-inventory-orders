using Erp.Application.Common.Models;
using Erp.Application.Inventory.StockMovements;
using Microsoft.EntityFrameworkCore;

namespace Erp.Infrastructure.Persistence.Queries
{
    public sealed class StockMovementQueries : IStockMovementQueries
    {
        private readonly AppDbContext _db;
        public StockMovementQueries(AppDbContext db) => _db = db;

        public async Task<PagedResult<StockMovementDto>> GetPagedProductAsync(Guid productId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _db.StockMovements.AsNoTracking().Where(m => m.ProductId == productId);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(m => m.CreatedAtUtc)
                .ThenBy(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new StockMovementDto(m.Id, m.ProductId, m.Type, m.QuantityChange, m.BalanceAfter, m.Reason, m.PerformedByUserId, m.CreatedAtUtc))
                .ToListAsync(cancellationToken);

            return new PagedResult<StockMovementDto>(items, total, page, pageSize);
        }
    }
}
