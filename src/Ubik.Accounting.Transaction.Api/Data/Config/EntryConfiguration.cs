﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Transaction.Api.Models;
using System.Globalization;

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

            builder.Property(a => a.Label)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .HasMaxLength(700);

            builder.Property(a => a.Amount)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.OwnsOne(a => a.AmountAdditionnalInfo, exchangeInfo =>
            {
                exchangeInfo.Ignore(t => t.OriginalCurrencyId);
                exchangeInfo.Ignore(t => t.ExchangeRate);
                exchangeInfo.Ignore(t => t.OriginalAmount);

                exchangeInfo.Property("_originalAmount")
                    .HasColumnName("original_amount")
                    .HasPrecision(18, 4)
                    .IsRequired(false);

                exchangeInfo.Property("_exchangeRate")
                    .HasColumnName("exchange_rate")
                    .HasPrecision(18, 10)
                    .IsRequired(false);

                exchangeInfo.Property("_originalCurrencyId")
                    .HasColumnName("original_currency_id")
                    .IsRequired(false);

                exchangeInfo.HasOne<Currency>()
                    .WithMany()
                    .HasForeignKey("_originalCurrencyId")
                    .IsRequired(false);
            });

            builder.OwnsOne(a => a.TaxInfo, taxInfo =>
            {
                taxInfo.Ignore(t => t.TaxAppliedRate);

                taxInfo.Property("_taxAppliedRate")
                    .HasColumnName("tax_applied_rate")
                    .HasPrecision(8, 5)
                    .IsRequired(false);

                taxInfo.Ignore(t => t.TaxRateId);

                taxInfo.Property("_taxRateId")
                    .HasColumnName("tax_rate_id")
                    .IsRequired(false);
            });

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