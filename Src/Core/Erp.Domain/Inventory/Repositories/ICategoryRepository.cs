namespace Erp.Domain.Inventory.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> HasProductAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task AddAsync(Category category,  CancellationToken cancellationToken = default);
        void Remove(Category category);
    }
}
