using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Data.Config
{
    public class UserRoleByTenantConfiguration : IEntityTypeConfiguration<UserRoleByTenant>
    {
        public void Configure(EntityTypeBuilder<UserRoleByTenant> builder)
        {
            builder.ToTable("users_roles_by_tenant");

            builder.Property(a => a.UserTenantId)
                .IsRequired();

            builder.Property(a => a.RoleId)
                .IsRequired();

            builder.HasIndex(a => new { a.UserTenantId, a.RoleId })
                .IsUnique();

            builder
            .HasOne(e => e.UserTenant)
            .WithMany()
            .HasForeignKey(e => e.UserTenantId).OnDelete(DeleteBehavior.Cascade);

            builder
           .HasOne(e => e.Role)
           .WithMany()
           .HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
