using Erp.Domain.Sales.Orders;
using Erp.Domain.Sales.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Erp.Infrastructure.Persistence.Repositories
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _db;
        public OrderRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
            => await _db.Orders.AddAsync(order, cancellationToken);

        public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}
