using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Config
{
    public class AccountVatConfigConfiguration : IEntityTypeConfiguration<AccountVatConfig>
    {
        public void Configure(EntityTypeBuilder<AccountVatConfig> builder)
        {
            builder.Property(a => a.Version)
                .IsConcurrencyToken();

            builder.Property(a => a.TenantId)
                .IsRequired();

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.CreatedBy)
                .IsRequired();

            builder.HasIndex(a => new { a.AccountId, a.VatRateId })
                .IsUnique();

            builder.HasIndex(a => a.TenantId);

            builder
            .HasOne(s => s.Account)
            .WithMany()
            .HasForeignKey(e => e.AccountId)
            .IsRequired(true);

            builder
            .HasOne(s => s.VatRate)
            .WithMany()
            .HasForeignKey(e => e.VatRateId)
            .IsRequired(true);

            builder
             .HasOne(s => s.VatAccount)
            .WithMany()
            .HasForeignKey(e => e.VatAccountId)
            .IsRequired(true);
        }
    }
}
