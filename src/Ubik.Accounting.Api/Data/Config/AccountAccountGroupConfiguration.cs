using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Config
{
    public class AccountAccountGroupConfiguration : IEntityTypeConfiguration<AccountAccountGroup>
    {
        public void Configure(EntityTypeBuilder<AccountAccountGroup> builder)
        {
            builder.Property(a => a.AccountId)
                .IsRequired();

            builder.Property(a => a.AccountGroupId)
                .IsRequired();

            builder.HasIndex(a => new { a.AccountGroupId, a.AccountId })
                .IsUnique();

            builder.HasIndex(a => a.TenantId);

            //TODO: Change that quick with userservice
            builder
                .HasQueryFilter(a => a.TenantId == Guid.Parse("727449e8-e93c-49e6-a5e5-1bf145d3e62d"));

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
        }
    }
}
