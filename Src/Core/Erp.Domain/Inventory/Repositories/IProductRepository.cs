using Erp.Domain.ValueObjects;

namespace Erp.Domain.Inventory.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> SkuExistsAsync(Sku sku, CancellationToken cancellationToken = default);
        Task AddAsync (Product product, CancellationToken cancellationToken = default);
        void Remove(Product product);
    }
}
