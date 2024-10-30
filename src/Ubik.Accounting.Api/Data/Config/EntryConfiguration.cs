using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Config
{
    public class EntryConfiguration : IEntityTypeConfiguration<Entry>
    {
        public void Configure(EntityTypeBuilder<Entry> builder)
        {
            builder.ToTable("entries");

            builder.Property(a => a.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(a => a.Sign)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(a => a.Label)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .HasMaxLength(700);

            builder.Property(a => a.Amount)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(a => a.OriginalAmount)
                .HasPrecision(18, 4);

            builder.Property(a => a.ExchangeRate)
                .HasPrecision(18, 10);

            builder.Property(a => a.Version)
                .IsConcurrencyToken();

            builder.Property(a => a.TenantId)
                .IsRequired();

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.CreatedBy)
                .IsRequired();

            builder.HasIndex(a => a.TenantId);

            builder
            .HasOne(s => s.Transaction)
            .WithMany()
            .HasForeignKey(e => e.TransactionId)
            .IsRequired(true);

            builder
            .HasOne(s => s.Account)
            .WithMany()
            .HasForeignKey(e => e.AccountId)
            .IsRequired(true);

            builder
            .HasOne(s => s.OriginalCurrency)
            .WithMany()
            .HasForeignKey(e => e.OriginalCurrencyId)
            .IsRequired(true);
        }
    }
}
