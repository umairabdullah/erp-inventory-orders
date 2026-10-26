using Erp.Domain.Inventory;
using Erp.Domain.Inventory.Repositories;
using Erp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Erp.Infrastructure.Persistence.Repositories
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _db;
        public ProductRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
            => await _db.Products.AddAsync(product, cancellationToken);

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public void Remove(Product product) => _db.Products.Remove(product);

        public Task<bool> SkuExistsAsync(Sku sku, CancellationToken cancellationToken = default)
            => _db.Products.AnyAsync(p => p.Sku == sku, cancellationToken);
    }
}
