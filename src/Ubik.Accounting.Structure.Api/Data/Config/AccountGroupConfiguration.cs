using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Structure.Api.Models;

namespace Ubik.Accounting.Structure.Api.Data.Config
{
    public class AccountGroupConfiguration : IEntityTypeConfiguration<AccountGroup>
    {
        public void Configure(EntityTypeBuilder<AccountGroup> builder)
        {
            builder.ToTable("account_groups");

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

            builder.HasIndex(a => new { a.Code, a.ClassificationId })
            .IsUnique();

            builder.HasIndex(a => a.TenantId);

            builder
                .HasOne(s => s.ParentAccountGroup)
                .WithMany(m => m.ChildrenAccountGroups)
                .HasForeignKey(e => e.ParentAccountGroupId).OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder
                .HasOne(a => a.Classification)
                .WithMany(g => g.OwnedAccountGroups)
                .HasForeignKey(b => b.ClassificationId).OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);
        }
    }
}
