﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Config
{
    public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("Currencies");

            builder.Property(a => a.IsoCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(a => a.Version)
                .IsConcurrencyToken();

            builder.Property(a => a.TenantId)
                .IsRequired();

            builder.HasIndex(a => new { a.IsoCode, a.TenantId })
                .IsUnique();

            builder.HasIndex(a => a.TenantId);
        }
    }
}
