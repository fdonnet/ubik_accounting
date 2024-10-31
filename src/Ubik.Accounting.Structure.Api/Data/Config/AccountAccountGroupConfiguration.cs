using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

            builder.HasIndex(a => a.TenantId);

            builder
            .HasOne(e => e.Account)
            .WithMany()
            .HasForeignKey(e => e.AccountId).OnDelete(DeleteBehavior.Cascade);

            builder
           .HasOne(e => e.AccountGroup)
           .WithMany()
           .HasForeignKey(e => e.AccountGroupId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
