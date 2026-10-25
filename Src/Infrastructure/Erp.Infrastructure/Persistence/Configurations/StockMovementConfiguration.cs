using Erp.Domain.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Erp.Infrastructure.Persistence.Configurations
{
    public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
    {
        public void Configure(EntityTypeBuilder<StockMovement> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedNever();

            builder.Property(m => m.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(m => m.Reason).HasMaxLength(500);

            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(m => m.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(m => new { m.ProductId, m.CreatedAtUtc });
        }
    }
}
