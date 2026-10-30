using Erp.Domain.Sales.Customers;
using Erp.Domain.Sales.Repositories;
using Erp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Erp.Infrastructure.Persistence.Repositories
{
    public sealed class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _db;
        public CustomerRepository(AppDbContext db) => _db = db;


        public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
            => await _db.Customers.AddAsync(customer, cancellationToken);

        public Task<bool> EmailExistsAsync(Email email, Guid? excludingCustomerId = null, CancellationToken cancellationToken = default)
            => _db.Customers.AnyAsync(c => c.Email == email && (excludingCustomerId == null || c.Id != excludingCustomerId), cancellationToken);    

        public  Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _db.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
