using Erp.Domain.Inventory;
using Erp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Erp.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);

            builder.Property(p => p.Sku)
                .HasConversion(sku => sku.Value, value => Sku.Create(value))
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(p => p.Sku).IsUnique();
            builder.OwnsOne(p => p.Price, price =>
            {
                price.Property(m => m.Amount).HasColumnName("PriceAmount").HasPrecision(18, 2).IsRequired();
                price.Property(m => m.Currency).HasColumnName("PriceCurrency").HasMaxLength(3).IsRequired();
            });

            builder.Navigation(p => p.Price).IsRequired();
            builder.Ignore(p => p.IsLowStock);

            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey( p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
