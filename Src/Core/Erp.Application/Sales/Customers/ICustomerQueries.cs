using Erp.Application.Common.Models;

namespace Erp.Application.Sales.Customers
{
    public interface ICustomerQueries
    {
        Task<PagedResult<CustomerDto>> GetPagedAsync(string? search, bool? isActive, int page, int pageSize, CancellationToken cancellationToken = default);

        Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
