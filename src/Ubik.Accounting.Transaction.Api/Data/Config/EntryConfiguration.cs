using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Transaction.Api.Models;

namespace Ubik.Accounting.Transaction.Api.Data.Config
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

            builder.HasOne<Tx>()
                .WithMany()
                .HasForeignKey(a => a.TxId).OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);

            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(a => a.AccountId).OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            //TODO: see that tomorrow... I want to be able to enrich the model and link it
            // to the DB in a smart way.
            builder.OwnsOne(a => a.TaxInfo, taxInfo =>
            {
                taxInfo.Property(t => t.TaxAppliedRate)
                    .HasPrecision(8, 5)
                    .IsRequired(false)
                    .HasConversion<decimal?>();

                taxInfo.Property(t => t.TaxRateId)
                    .IsRequired(false);
            });

            //builder.OwnsOne(a => a.AmountExchangeInfo, exchangeInfo =>
            //{
            //    exchangeInfo.Property(t => t.OriginalAmount)
            //        .HasPrecision(18, 4)
            //        .IsRequired(false);

            //    exchangeInfo.Property(t => t.ExchangeRate)
            //        .HasPrecision(18, 10)
            //        .IsRequired(false);

            //    exchangeInfo.HasOne<Currency>()
            //        .WithMany()
            //        .HasForeignKey(e => e.OriginalCurrencyId)
            //        .IsRequired(false);
            //});

            builder.Property(a => a.Label)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .HasMaxLength(700);

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
        }
    }
}
