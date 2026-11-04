using Erp.Application.Common.Models;
using Erp.Application.Inventory.Users;
using Microsoft.EntityFrameworkCore;

namespace Erp.Infrastructure.Persistence.Queries
{
    public sealed class UserQueries : IUserQueries
    {
        private readonly AppDbContext _db;
        public UserQueries(AppDbContext db) => _db = db;
        

        public async Task<PagedResult<UserListItemDto>> GetPagedAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _db.Users.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(u => u.FullName.Contains(term));
            }

            var total = await _db.Users.CountAsync(cancellationToken);
            var users = await query
                .OrderBy(u => u.FullName)
                .ThenBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var items = users.Select(u => new UserListItemDto(
                 u.Id, u.FullName, u.Email.Value, u.Role.ToString(), u.IsActive, u.EmailConfirmed)).ToList();

            return new PagedResult<UserListItemDto>(items, total, page, pageSize);
        }
    }
}
