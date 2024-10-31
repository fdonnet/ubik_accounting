using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Config
{
    public class VatRateConfiguration : IEntityTypeConfiguration<VatRate>
    {
        public void Configure(EntityTypeBuilder<VatRate> builder)
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

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.CreatedBy)
                .IsRequired();

            builder.HasIndex(a => new { a.Code, a.TenantId })
                .IsUnique();

            builder.HasIndex(a => a.TenantId);

            builder.HasIndex(a => a.ValidFrom);
            builder.HasIndex(a => a.ValidTo);
        }
    }
}
