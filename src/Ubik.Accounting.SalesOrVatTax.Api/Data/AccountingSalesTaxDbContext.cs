using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.SalesOrVatTax.Api.Data.Config;
using Ubik.Accounting.SalesOrVatTax.Api.Models;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Services;
using Ubik.DB.Common.Extensions;

namespace Ubik.Accounting.SalesOrVatTax.Api.Data
{
    public class AccountingSalesTaxDbContext(DbContextOptions<AccountingSalesTaxDbContext> options
        , ICurrentUser currentUser) : DbContext(options)
    {
        public DbSet<TaxRate> SalesOrVatTaxRates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSnakeCaseNamingConvention();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                var err = new CustomError()
                {
                    ErrorCode = "DB_CONCURRENCY_CONFLICT",
                    ErrorFriendlyMessage = "You don't have the last version or the ressource, refresh your data before updating.",
                    ErrorValueDetails = "Version",
                };
                var conflict = new UpdateDbConcurrencyException()
                {
                    CustomErrors = [err]
                };

                throw conflict;
            }
        }

        public void SetAuditAndSpecialFields()
        {
            ChangeTracker.SetSpecialFields(currentUser);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Build for Masstransit inbox/outbox
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();

            //TenantId
            SetTenantId(modelBuilder);

            //Configure
            new TaxRateConfiguration().Configure(modelBuilder.Entity<TaxRate>());


            base.OnModelCreating(modelBuilder);
        }

        private void SetTenantId(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxRate>()
                .HasQueryFilter(mt => mt.TenantId == currentUser.TenantId);
        }
    }
}
