using Erp.Application.Common.Models;
using Erp.Application.Inventory.Products;
using Erp.Domain.Inventory;
using Erp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

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
            var query = Joined();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(r => r.Product.Name.Contains(term));
            }

            if (categoryId.HasValue)
                query = query.Where(r => r.Product.CategoryId ==  categoryId.Value);

            var total = await query.CountAsync(cancellationToken);

            var rows = await query
                .OrderBy(r => r.Product.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(Project)
                .ToListAsync(cancellationToken);

            return new PagedResult<ProductDto>(rows.Select(ToDto).ToList(), total, page, pageSize);
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var row = await Joined()
                .Where(r => r.Product.Id == id)
                .Select(Project)
                .FirstOrDefaultAsync(cancellationToken);
            return row is null ? null : ToDto(row);
        }

        private IQueryable<ProductJoin> Joined() =>
            from p in _db.Products.AsNoTracking()
            join c in _db.Categories.AsNoTracking()
            on p.CategoryId equals c.Id
            select new ProductJoin { Product = p, CategoryName = c.Name };


        private static readonly Expression<Func<ProductJoin, ProductRow>> Project = x => new ProductRow(
       x.Product.Id, x.Product.Name, x.Product.Description, x.Product.Sku,
       x.Product.Price.Amount, x.Product.Price.Currency,
       x.Product.QuantityInStock, x.Product.ReorderLevel, x.Product.IsActive,
       x.Product.CategoryId, x.CategoryName);


        private static ProductDto ToDto(ProductRow r) => new(
         r.Id, r.Name, r.Description, r.Sku.Value,
         r.PriceAmount, r.PriceCurrency,
         r.QuantityInStock, r.ReorderLevel,
         r.QuantityInStock <= r.ReorderLevel,
         r.IsActive, r.CategoryId, r.CategoryName);


        private sealed class ProductJoin
        {
            public Product Product { get; set; } = null!;
            public string CategoryName { get; set; } = string.Empty;
        }
        private sealed record ProductRow(Guid Id, string Name, string? Description, Sku Sku, decimal PriceAmount, string PriceCurrency, int QuantityInStock, int ReorderLevel, bool IsActive,
           Guid CategoryId, string CategoryName);
    }


  
}
