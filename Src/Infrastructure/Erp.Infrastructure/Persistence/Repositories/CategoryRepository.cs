using Erp.Domain.Inventory;
using Erp.Domain.Inventory.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Erp.Infrastructure.Persistence.Repositories
{
    public sealed class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _db;
        public CategoryRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
            => await _db.Categories.AddAsync(category, cancellationToken);


        public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _db.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public Task<bool> HasProductAsync(Guid categoryId, CancellationToken cancellationToken = default)
            => _db.Products.AnyAsync(p => p.CategoryId == categoryId, cancellationToken);

        public Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            var trimmed = name.Trim();
            return _db.Categories.AnyAsync(c => c.Name == trimmed, cancellationToken);
        }

        public void Remove(Category category) => _db.Categories.Remove(category);
        
    }
}
