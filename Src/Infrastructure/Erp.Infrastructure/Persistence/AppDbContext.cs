using Erp.Domain.Common;
using Erp.Domain.Identity;
using Erp.Domain.Inventory;
using Erp.Domain.Sales.Customers;
using Erp.Domain.Sales.Orders;
using Microsoft.EntityFrameworkCore;
using Erp.Application.Common.Exceptions;
using Erp.Application.Common.Interfaces;

namespace Erp.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventDispatcher _dispatcher;
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions, IDomainEventDispatcher dispatcher) : base(dbContextOptions)
        {
            _dispatcher = dispatcher;
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        public DbSet<StockMovement> StockMovements => Set<StockMovement>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<IDomainEvent>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var aggregates = ChangeTracker.Entries<AggregateRoot>()
                .Select(e => e.Entity)
                .Where(a => a.DomainEvents.Count > 0)
                .ToList();

            var events = aggregates.SelectMany(a => a.DomainEvents).ToList();
            int result;
            try
            {
                result = await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {

                throw new ConflictException("The data was change by someone else while you were working, please reload and try again.");
            }

            foreach (var aggregate in aggregates)
                aggregate.ClearDomainEvents();

            await _dispatcher.DispatchAsync(events, cancellationToken);
            return result;
        }
    }
}
