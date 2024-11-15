using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Data.Config
{
    public class RoleAuthorizationConfiguration : IEntityTypeConfiguration<RoleAuthorization>
    {
        public void Configure(EntityTypeBuilder<RoleAuthorization> builder)
        {
            builder.ToTable("roles_authorizations");

            builder.Property(a => a.RoleId)
                .IsRequired();

            builder.Property(a => a.AuthorizationId)
                .IsRequired();

            builder.HasIndex(a => new { a.RoleId, a.AuthorizationId })
                .IsUnique();

            builder.Property(a => a.Version)
                .IsConcurrencyToken();

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

            builder
            .HasOne<Role>()
            .WithMany()
            .HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Cascade);

            builder
           .HasOne<Authorization>()
           .WithMany()
           .HasForeignKey(e => e.AuthorizationId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
