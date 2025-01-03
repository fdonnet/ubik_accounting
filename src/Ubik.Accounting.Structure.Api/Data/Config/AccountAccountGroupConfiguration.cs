﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Structure.Api.Models;

namespace Ubik.Accounting.Structure.Api.Data.Config
{
    //TODO: manage the selected tenant and implement mandantor too for all the configs
    public class AccountAccountGroupConfiguration : IEntityTypeConfiguration<AccountAccountGroup>
    {
        public void Configure(EntityTypeBuilder<AccountAccountGroup> builder)
        {
            builder.ToTable("accounts_account_groups");

            builder.Property(a => a.AccountId)
                .IsRequired();

            builder.Property(a => a.AccountGroupId)
                .IsRequired();

            builder.HasIndex(a => new { a.AccountGroupId, a.AccountId })
                .IsUnique();

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

            builder.HasIndex(a => a.TenantId);

            builder
            .HasOne<Account>()
            .WithMany()
            .HasForeignKey(e => e.AccountId).OnDelete(DeleteBehavior.Cascade);

            builder
           .HasOne<AccountGroup>()
           .WithMany()
           .HasForeignKey(e => e.AccountGroupId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
