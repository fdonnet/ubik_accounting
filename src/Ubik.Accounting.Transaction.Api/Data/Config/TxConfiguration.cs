using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Transaction.Api.Models;

namespace Ubik.Accounting.Transaction.Api.Data.Config
{
    public class TxConfiguration : IEntityTypeConfiguration<Tx>
    {
        public void Configure(EntityTypeBuilder<Tx> builder)
        {
            builder.Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(x => x.ValueDate)
                .IsRequired();

            builder.Property(a => a.Version)
               .IsConcurrencyToken();

            builder.Property(a => a.TenantId)
               .IsRequired();

            builder.OwnsOne(x => x.State, stateInfo =>
            {
                stateInfo.Property(s => s.State)
                    .HasColumnName("state")
                    .HasConversion<int>()
                    .IsRequired();
                stateInfo.Property(s => s.Reason)
                    .HasColumnName("state_reason")
                    .HasMaxLength(400);

                stateInfo.HasIndex(x => x.State);
            });

            builder.Navigation(x => x.State)
                .IsRequired();

            builder.HasIndex(x => x.TenantId);

            builder.HasIndex(x => x.ValueDate);

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
        }
    }

}
