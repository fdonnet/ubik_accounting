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

            builder.HasIndex(a => a.Code)
                .IsUnique();

            builder.HasIndex(a => a.TenantId);

            //TODO: Change that quick with userservice
            builder
                .HasQueryFilter(a => a.TenantId == Guid.Parse("727449e8-e93c-49e6-a5e5-1bf145d3e62d"));

            //Relations
            builder
                .HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(b => b.CreatedBy)
                .IsRequired(true);

            builder
                .HasOne(a => a.ModifiedByUser)
                .WithMany()
                .HasForeignKey(b => b.ModifiedBy)
                .IsRequired(false);

            builder
                .HasOne(g => g.AccountGroup)
                .WithMany(g => g.Accounts)
                .HasForeignKey(x => x.AccountGroupId)
                .IsRequired(false);
        }
    }
}
