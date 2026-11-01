using Erp.Application.Common.Models;
using Erp.Application.Sales.Orders;
using Erp.Domain.Sales.Orders;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;

namespace Erp.Infrastructure.Persistence.Queries
{
    public sealed class OrderQueries : IOrderQueries
    {
        private readonly AppDbContext _db;
        public OrderQueries(AppDbContext db) => _db = db;

        public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var order = await _db.Orders.AsNoTracking()
                .Include(o => o.Items)
                .FirstOrDefaultAsync( o => o.Id == id, cancellationToken);

            if (order is null)
                return null;

            var customerName = await _db.Customers
                .AsNoTracking()
                .Where(c => c.Id == order.CustomerId)
                .Select(c => c.Name).FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

            var items = order.Items
                .OrderBy(i => i.ProductName)
                .Select(i => new OrderItemDto(
                i.ProductId, i.ProductName, i.ProductSku, i.UnitPrice.Amount, i.Quantity,i.LineTotal.Amount)).ToList();

            return new OrderDto(order.Id, order.OrderNumber, order.CustomerId, customerName, order.Status, order.PlacedAtUtc, order.Notes, order.CancellationReason, order.CreatedByUserId, order.Total.Amount, order.Total.Currency, items);

        }

        public async Task<PagedResult<OrderSummaryDto>> GetPagedAsync(string? search, OrderStatus? Status, Guid? customerId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = Joined();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(x => x.Order.OrderNumber.Contains(term) || x.CustomerName.Contains(term));
            }
            if (Status.HasValue)
                query = query.Where(x => x.Order.Status == Status.Value);

            if(customerId.HasValue)
                query = query.Where(x=> x.Order.CustomerId == customerId.Value);

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.Order.PlacedAtUtc)
                .ThenBy(x => x.Order.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new OrderSummaryDto(
                    x.Order.Id, x.Order.OrderNumber, x.Order.CustomerId, x.CustomerName,
                    x.Order.Status, x.Order.PlacedAtUtc,
                    x.Order.Total.Amount, x.Order.Total.Currency,
                    x.Order.Items.Count()))
                .ToListAsync(cancellationToken);

            return new PagedResult<OrderSummaryDto>(items, total, page, pageSize);
        }

        private IQueryable<OrderJoin> Joined() =>
            from o in _db.Orders.AsNoTracking()
            join c in _db.Customers.AsNoTracking() on o.CustomerId equals c.Id
            select new OrderJoin { Order = o, CustomerName = c.Name };
        private sealed class OrderJoin
        {
            public Order Order { get; set; } = null!;
            public string CustomerName { get; set; } = string.Empty;
        }
    }
}
