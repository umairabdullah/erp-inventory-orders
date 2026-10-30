using Erp.Domain.Sales.Customers;
using Erp.Domain.ValueObjects;

namespace Erp.Domain.Sales.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(Email email, Guid? excludingCustomerId = null, CancellationToken cancellationToken = default);

        Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    }
}
