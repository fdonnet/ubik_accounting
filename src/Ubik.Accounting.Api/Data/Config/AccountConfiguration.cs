using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Config
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

            builder.HasIndex(a => new { a.Code, a.TenantId })
                .IsUnique();

            builder.HasIndex(a => a.TenantId);

            builder.Property(a => a.Domain)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(a => a.Category)
                .IsRequired()
                .HasConversion<int>();

            builder
                .HasOne(s => s.Currency)
                .WithMany()
                .HasForeignKey(e => e.CurrencyId)
                .IsRequired(true);
        }
    }
}
