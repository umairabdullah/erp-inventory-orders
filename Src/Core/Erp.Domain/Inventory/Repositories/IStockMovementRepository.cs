namespace Erp.Domain.Inventory.Repositories
{
    public interface IStockMovementRepository
    {
        Task AddAsync(StockMovement stockMovement, CancellationToken cancellationToken = default);
    }
}
