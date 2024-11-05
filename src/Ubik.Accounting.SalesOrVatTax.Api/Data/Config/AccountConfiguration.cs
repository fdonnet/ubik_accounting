using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.SalesOrVatTax.Api.Models;

namespace Ubik.Accounting.SalesOrVatTax.Api.Data.Config
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.Property(a => a.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(a => a.Label)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.TenantId)
                .IsRequired();

            builder.Property(a => a.Active)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(a => a.Version)
                .IsConcurrencyToken();

            builder.HasIndex(a => new { a.Code, a.TenantId })
                .IsUnique();

            builder.HasIndex(a => a.TenantId);
        }
    }
}
