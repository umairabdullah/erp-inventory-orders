using Erp.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erp.Application.Inventory.Categories
{
    public interface ICategoryQueries 
    {
        Task<PagedResult<CategoryDto>> GetPagedAsync(string? search , int page, int pageSize, CancellationToken cancellationToken = default);

        Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
