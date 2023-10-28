using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Config
{
    public class AccountGroupClassificationConfiguration : IEntityTypeConfiguration<AccountGroupClassification>
    {
        public void Configure(EntityTypeBuilder<AccountGroupClassification> builder)
        {
            builder.ToTable("AccountGroupClassifications");

            builder.Property(a => a.Code)
           .IsRequired()
           .HasMaxLength(20);

            builder.Property(a => a.Label)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .HasMaxLength(700);

            builder.Property(a => a.Version)
            .IsConcurrencyToken();

            builder.Property(a => a.TenantId)
                .IsRequired();

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.CreatedBy)
                .IsRequired();

            builder.HasIndex(a => a.Code)
            .IsUnique();

            builder.HasIndex(a => a.TenantId);
        }
    }
}
