using Erp.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Erp.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {

            builder.ToTable("RefreshTokens");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).ValueGeneratedNever();

            builder.Property(r => r.TokenHash).IsRequired().HasMaxLength(128);
            builder.Property(r => r.TokenHash).IsUnicode();

            builder.Property(r => r.ReplacedByTokenHash).HasMaxLength(128);
            builder.Ignore(r => r.IsRevoked);
        }
    }
}
