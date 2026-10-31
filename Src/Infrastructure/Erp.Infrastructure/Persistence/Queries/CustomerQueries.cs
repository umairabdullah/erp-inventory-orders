using Erp.Application.Common.Models;
using Erp.Application.Sales.Customers;
using Erp.Domain.Sales.Customers;
using Microsoft.EntityFrameworkCore;

namespace Erp.Infrastructure.Persistence.Queries
{
    public sealed class CustomerQueries : ICustomerQueries
    {
        private readonly AppDbContext _db;
        public CustomerQueries(AppDbContext db) => _db = db;


        public async Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var customer = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id , cancellationToken);

            return customer is null ? null : ToDto(customer);
        }

      

        public async Task<PagedResult<CustomerDto>> GetPagedAsync(string? search, bool? isActive, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query =  _db.Customers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(s => s.Name.Contains(term));

                //query = query.Where(c => EF.Functions.Collate(c.Name, "SQL_Latin1_General_CP1_CI_AS").Contains(term));
            }

            if(isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            //var sql = query.ToQueryString();

            var total = await query.CountAsync(cancellationToken);

            var customers = await query
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<CustomerDto>(customers.Select(ToDto).ToList(), total, page, pageSize);

        }

        private static CustomerDto ToDto(Customer c) =>
            new(
              c.Id, c.Name, c.Email.Value, c.Phone,
              new AddressDto(c.Address.Street, c.Address.City, c.Address.State, c.Address.PostalCode, c.Address.Country),
              c.IsActive);
    }
}
