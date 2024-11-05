using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ubik.Accounting.SalesOrVatTax.Api.Models;

namespace Ubik.Accounting.SalesOrVatTax.Api.Data.Config
{
    public class AccountTaxRateConfigConfiguration : IEntityTypeConfiguration<AccountTaxRateConfig>
    {
        public void Configure(EntityTypeBuilder<AccountTaxRateConfig> builder)
        {
            builder.Property(a => a.Version)
                .IsConcurrencyToken();

            builder.Property(a => a.TenantId)
                .IsRequired();

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.CreatedBy)
                .IsRequired();

            builder.HasIndex(a => new { a.AccountId, a.TaxRateId })
                .IsUnique();

            builder.HasIndex(a => a.TenantId);

            builder
            .HasOne<Account>()
            .WithMany()
            .HasForeignKey(e => e.AccountId)
            .IsRequired(true);

            builder
            .HasOne<TaxRate>()
            .WithMany()
            .HasForeignKey(e => e.TaxRateId)
            .IsRequired(true);

            builder
            .HasOne<Account>()
            .WithMany()
            .HasForeignKey(e => e.TaxAccountId)
            .IsRequired(true);

        }
    }
}
