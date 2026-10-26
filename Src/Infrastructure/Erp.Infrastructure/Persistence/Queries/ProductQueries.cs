using Erp.Application.Common.Models;
using Erp.Application.Inventory.Products;
using Erp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Erp.Infrastructure.Persistence.Queries
{
    public sealed class ProductQueries : IProductQueries
    {
        private readonly AppDbContext _db;
        public ProductQueries(AppDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResult<ProductDto>> GetPagedAsync(string? search, Guid? categoryId, int page , int pageSize, CancellationToken cancellationToken = default)
        {
            var query = Rows();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(r => r.Name.Contains(term));
            }

            if (categoryId.HasValue)
                query = query.Where(r => r.CategoryId ==  categoryId.Value);

            var total = await query.CountAsync(cancellationToken);

            var rows = await query
                .OrderBy(r => r.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<ProductDto>(rows.Select(ToDto).ToList(), total, page, pageSize);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var row = await Rows().FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
            return row is null ? null : ToDto(row);
        }

        private IQueryable<ProductRow> Rows() =>
            from p in _db.Products.AsNoTracking()
            join c in _db.Categories.AsNoTracking()
            on p.CategoryId equals c.Id
            select new ProductRow(
                p.Id, p.Name, p.Description, p.Sku, p.Price.Amount, p.Price.Currency, p.QuantityInStock,
                p.ReorderLevel, p.IsActive, p.CategoryId, c.Name);

        private static ProductDto ToDto(ProductRow r) => new(
         r.Id, r.Name, r.Description, r.Sku.Value,
         r.PriceAmount, r.PriceCurrency,
         r.QuantityInStock, r.ReorderLevel,
         r.QuantityInStock <= r.ReorderLevel,
         r.IsActive, r.CategoryId, r.CategoryName);

        private sealed record ProductRow(Guid Id, string Name, string? Description, Sku Sku, decimal PriceAmount, string PriceCurrency, int QuantityInStock, int ReorderLevel, bool IsActive,
           Guid CategoryId, string CategoryName);
    }


  
}
