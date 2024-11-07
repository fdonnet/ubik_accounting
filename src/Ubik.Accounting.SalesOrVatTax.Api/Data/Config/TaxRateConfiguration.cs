using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.SalesOrVatTax.Api.Models;

namespace Ubik.Accounting.SalesOrVatTax.Api.Data.Config
{
    public class TaxRateConfiguration : IEntityTypeConfiguration<TaxRate>
    {
        public void Configure(EntityTypeBuilder<TaxRate> builder)
        {
            builder.Property(a => a.Code)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(a => a.Description)
                .HasMaxLength(200);

            builder.Property(a => a.Rate)
                .HasPrecision(8, 5);

            builder.Property(a => a.Version)
                .IsConcurrencyToken();

            builder.Property(a => a.TenantId)
                .IsRequired();

            builder.OwnsOne(x => x.AuditInfo, auditInfo =>
            {
                auditInfo.Property(a => a.ModifiedAt)
                    .HasColumnName("modified_at")
                    .IsRequired();

                auditInfo.Property(a => a.ModifiedBy)
                    .HasColumnName("modified_by")
                    .IsRequired();

                auditInfo.Property(a => a.CreatedAt)
                    .HasColumnName("created_at")
                    .IsRequired();

                auditInfo.Property(a => a.CreatedBy)
                    .HasColumnName("created_by")
                    .IsRequired();
            });

            builder.HasIndex(a => new { a.Code, a.TenantId })
                .IsUnique();

            builder.HasIndex(a => a.TenantId);

            builder.HasIndex(a => a.ValidFrom);
            builder.HasIndex(a => a.ValidTo);
        }
    }
}
