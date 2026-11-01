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

        public async Task<IReadOnlyList<Product>> GetByIdAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
        {
            var idList = ids.ToList();
            return await _db.Products.Where(p => idList.Contains(p.Id)).ToListAsync(cancellationToken);
        }

        public void Remove(Product product) => _db.Products.Remove(product);

        public Task<bool> SkuExistsAsync(Sku sku, CancellationToken cancellationToken = default)
        {
            return _db.Products.AnyAsync(p => p.Sku == sku, cancellationToken);
        }
    }
}
