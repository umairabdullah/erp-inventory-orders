using Erp.Domain.Sales.Customers;
using Erp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Erp.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {

        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();

            builder.Property(c => c.Name).IsRequired().HasMaxLength(150);

            builder.Property(c => c.Email)
                .HasConversion(email => email.Value, value => Email.Create(value))
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(c => c.Email).IsUnique();
            builder.Property(c => c.Phone).HasMaxLength(30);

            builder.OwnsOne(c => c.Address, address =>
            {
                address.Property(a => a.Street).HasColumnName("AddressStreet").IsRequired().HasMaxLength(200);
                address.Property(a => a.City).HasColumnName("AddressCity").IsRequired().HasMaxLength(100);
                address.Property(a => a.State).HasColumnName("AddressState").HasMaxLength(100);
                address.Property(a => a.PostalCode).HasColumnName("AddressPostalCode").IsRequired().HasMaxLength(20);
                address.Property(a => a.Country).HasColumnName("AddressCountry").IsRequired().HasMaxLength(100);
            });

            builder.Navigation(c => c.Address).IsRequired();

        }
    }
}
