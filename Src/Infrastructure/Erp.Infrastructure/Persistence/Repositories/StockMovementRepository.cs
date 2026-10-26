using Erp.Domain.Inventory;
using Erp.Domain.Inventory.Repositories;

namespace Erp.Infrastructure.Persistence.Repositories
{
    public sealed class StockMovementRepository : IStockMovementRepository
    {
        private readonly AppDbContext _db;
        public StockMovementRepository(AppDbContext db) => _db = db;
       
        public async Task AddAsync(StockMovement stockMovement, CancellationToken cancellationToken = default)
            => await _db.AddAsync(stockMovement, cancellationToken);
        
    }
}
