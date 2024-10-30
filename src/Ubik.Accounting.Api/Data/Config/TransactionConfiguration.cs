using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Config
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.Property(a => a.Label)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Amount)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(a => a.Version)
               .IsConcurrencyToken();

            builder.Property(a => a.TenantId)
                .IsRequired();

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.CreatedBy)
                .IsRequired();

            builder.HasIndex(a => a.TenantId);

            builder.HasIndex(a => a.ValueDate);
        }
    }
}
