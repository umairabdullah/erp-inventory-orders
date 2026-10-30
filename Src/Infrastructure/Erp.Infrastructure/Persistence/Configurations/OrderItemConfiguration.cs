using Erp.Domain.Inventory;
using Erp.Domain.Sales.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Erp.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(x => x.OrderId);
            builder.Property(x => x.OrderId).ValueGeneratedNever();

            builder.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
            builder.Property(i => i.ProductSku).IsRequired().HasMaxLength(30);

            builder.OwnsOne(i => i.UnitPrice, price =>
            {
                price.Property(m => m.Amount).HasColumnName("UnitPriceAmount").HasPrecision(18, 2).IsRequired();
                price.Property(m => m.Currency).HasColumnName("UnitPriceCurrency").HasMaxLength(3).IsRequired();

            });

            builder.Navigation(i => i.UnitPrice).IsRequired();
            builder.Ignore(l => l.LineTotal);

            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(i => new { i.OrderId, i.ProductId }).IsUnique();
        }
    }
}
