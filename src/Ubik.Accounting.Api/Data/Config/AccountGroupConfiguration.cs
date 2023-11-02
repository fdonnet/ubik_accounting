using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Config
{
    public class AccountGroupConfiguration : IEntityTypeConfiguration<AccountGroup>
    {
        public void Configure(EntityTypeBuilder<AccountGroup> builder)
        {
            builder.ToTable("AccountGroups");

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

            builder.HasIndex(a => new { a.Code, a.AccountGroupClassificationId })
            .IsUnique();

            builder.HasIndex(a => a.TenantId);

            builder
                .HasOne(s => s.ParentAccountGroup)
                .WithMany(m => m.ChildrenAccountGroups)
                .HasForeignKey(e => e.ParentAccountGroupId)
                .IsRequired(false);

            builder
                .HasOne(a => a.AccountGroupClassification)
                .WithMany(g => g.OwnedAccountGroups)
                .HasForeignKey(b => b.AccountGroupClassificationId).OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);

            builder
                .HasMany(e => e.Accounts)
                .WithMany()
                .UsingEntity<AccountAccountGroup>(
                a => a.HasOne<Account>().WithMany().HasForeignKey(e => e.AccountId).OnDelete(DeleteBehavior.Cascade),
                g => g.HasOne<AccountGroup>().WithMany().HasForeignKey(e => e.AccountGroupId).OnDelete(DeleteBehavior.Cascade));
        }
    }
}
