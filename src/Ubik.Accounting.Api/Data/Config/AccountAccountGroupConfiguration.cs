using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Services;

namespace Ubik.Accounting.Api.Data.Config
{
    //TODO: manage the selected tenant and implement mandantor too for all the configs
    public class AccountAccountGroupConfiguration : IEntityTypeConfiguration<AccountAccountGroup>
    {
        public void Configure(EntityTypeBuilder<AccountAccountGroup> builder)
        {
            builder.ToTable("AccountsAccountGroups");

            builder.Property(a => a.AccountId)
                .IsRequired();

            builder.Property(a => a.AccountGroupId)
                .IsRequired();

            builder.HasIndex(a => new { a.AccountGroupId, a.AccountId })
                .IsUnique();

            builder.HasIndex(a => a.TenantId);
        }
    }
}
