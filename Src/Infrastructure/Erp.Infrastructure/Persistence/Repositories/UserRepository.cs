using Erp.Domain.Identity;
using Erp.Domain.Identity.Repositories;
using Erp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Erp.Infrastructure.Persistence.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _db.AddAsync(user, cancellationToken);
        }

        public  Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default)
        {
            return _db.Users.AnyAsync(e => e.Email == email, cancellationToken);
        }

        public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return _db.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _db.Users.Include(r => r.RefreshTokens).FirstOrDefaultAsync(u => u.Id == id , cancellationToken);
        }

        public Task<User?> GetByRefreshTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        {
            return _db.Users.Include(r => r.RefreshTokens).FirstOrDefaultAsync(r => r.RefreshTokens.Any(t => t.TokenHash == tokenHash), cancellationToken);
        }
    }
}
