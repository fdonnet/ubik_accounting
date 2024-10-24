using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Data.Config
{
    public class UserTenantConfiguration : IEntityTypeConfiguration<UserTenant>
    {
        public void Configure(EntityTypeBuilder<UserTenant> builder)
        {
            builder.ToTable("users_tenants");

            builder.Property(a => a.UserId)
                .IsRequired();

            builder.Property(a => a.TenantId)
                .IsRequired();

            builder.HasIndex(a => new { a.UserId, a.TenantId })
                .IsUnique();

            builder.Property(a => a.Version)
                .IsConcurrencyToken();

            //TODO: very dangerous, change that
            builder
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);

            builder
           .HasOne(e => e.Tenant)
           .WithMany()
           .HasForeignKey(e => e.TenantId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
