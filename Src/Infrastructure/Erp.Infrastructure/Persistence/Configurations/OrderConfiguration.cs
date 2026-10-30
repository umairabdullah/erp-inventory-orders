using Erp.Domain.Sales.Customers;
using Erp.Domain.Sales.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Erp.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id).ValueGeneratedNever();

            builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(30);
            builder.HasIndex(o => o.OrderNumber).IsUnique();

            builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            builder.Property(o => o.Notes).HasMaxLength(500);
            builder.Property(o => o.CancellationReason).HasMaxLength(500);

            builder.OwnsOne(o => o.Total, total =>
            {
                total.Property(m => m.Amount).HasColumnName("TotalAmount").HasPrecision(18, 2).IsRequired();
                total.Property(m => m.Currency).HasColumnName("TotalCurrency").HasMaxLength(30).IsRequired();
            });

            builder.Navigation(o => o.Total).IsRequired();

            builder.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(i => i.OrderId) .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(o => o.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(o => o.PlacedAtUtc);
        }
    }
}
