using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Data.Config
{
    public class AuthorizationConfiguration : IEntityTypeConfiguration<Authorization>
    {
        public void Configure(EntityTypeBuilder<Authorization> builder)
        {
            builder.Property(a => a.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(a => a.Code)
                .IsUnique();

            builder.Property(a => a.Label)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .HasMaxLength(700);

            builder.Property(a => a.Version)
                .IsConcurrencyToken();

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.CreatedBy)
                .IsRequired();
        }
    }
}
