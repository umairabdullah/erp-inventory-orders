using Erp.Application.Common.Models;

namespace Erp.Application.Inventory.Users
{
    public interface IUserQueries
    {
        Task<PagedResult<UserListItemDto>> GetPagedAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
