using Erp.Application.Common.Models;

namespace Erp.Application.Inventory.Products
{
    public interface IProductQueries
    {
        Task<PagedResult<ProductDto>> GetPagedAsync(string? Search, Guid? categoryId, int page, int pageSize, CancellationToken cancellationToken = default);

        Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
