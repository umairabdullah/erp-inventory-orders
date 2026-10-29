using Erp.Domain.ValueObjects;

namespace Erp.Domain.Identity.Repositories
{
    public interface IUserRepository
    {

        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<User?> GetByRefreshTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);

    }
}
