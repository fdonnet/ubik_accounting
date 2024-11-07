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

            builder.HasIndex(a => new { a.UserTenantId, a.RoleId })
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

            builder.Property(a => a.Version)
                .IsConcurrencyToken();

            builder
            .HasOne<UserTenant>()
            .WithMany()
            .HasForeignKey(e => e.UserTenantId).OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

            builder
           .HasOne<Role>()
           .WithMany()
           .HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Cascade)
           .IsRequired();

        }
    }
}
