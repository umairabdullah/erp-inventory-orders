using Erp.Domain.Common;
using Erp.Domain.Inventory;
using Microsoft.EntityFrameworkCore;

namespace Erp.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IUnitOfWork
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();

        public DbSet<StockMovement> StockMovements => Set<StockMovement>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
