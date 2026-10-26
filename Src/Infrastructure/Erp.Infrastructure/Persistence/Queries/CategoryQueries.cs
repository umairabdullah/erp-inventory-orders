using Erp.Application.Common.Models;
using Erp.Application.Inventory.Categories;
using Microsoft.EntityFrameworkCore;

namespace Erp.Infrastructure.Persistence.Queries
{
    public sealed class CategoryQueries : ICategoryQueries
    {
        private readonly AppDbContext _db;
        public CategoryQueries(AppDbContext db) => _db = db;
        
        public Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _db.Categories
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto(c.Id, c.Name, c.Description))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedResult<CategoryDto>> GetPagedAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _db.Categories.AsNoTracking();
            if(!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(c => c.Name.Contains(term));
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CategoryDto(c.Id, c.Name, c.Description))
                .ToListAsync(cancellationToken);

            return new PagedResult<CategoryDto>(items, total, page, pageSize);
        }
    }
}
