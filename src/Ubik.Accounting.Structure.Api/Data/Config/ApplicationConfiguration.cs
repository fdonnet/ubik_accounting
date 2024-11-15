using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Structure.Api.Models;

namespace Ubik.Accounting.Structure.Api.Data.Config
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.ToTable("application", t =>
            {
                t.HasCheckConstraint("ck_application_is_ready", "id = 1");
            });

            builder.Property(a => a.IsReady)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasData(new Application
            {
                Id = 1,
                IsReady = false
            });
        }
    }
}
