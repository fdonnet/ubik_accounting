using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Transaction.Api.Data.Config;
using Ubik.Accounting.Transaction.Api.Models;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Services;
using Ubik.DB.Common.Extensions;

namespace Ubik.Accounting.Transaction.Api.Data
{
    public class AccountingTxContext(DbContextOptions<AccountingTxContext> options
        , ICurrentUser userService) : DbContext(options)
    {
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<TaxRate> TaxRates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSnakeCaseNamingConvention();
        }

        //TODO: need to implement something for a basic standard DBContex conf.
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
            ChangeTracker.SetSpecialFields(userService);
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
            new EntryConfiguration().Configure(modelBuilder.Entity<Entry>());
            new AccountConfiguration().Configure(modelBuilder.Entity<Account>());
            new TaxRateConfiguration().Configure(modelBuilder.Entity<TaxRate>());
            //new AccountGroupConfiguration().Configure(modelBuilder.Entity<AccountGroup>());
            //new AccountConfiguration().Configure(modelBuilder.Entity<Account>());
            //new AccountAccountGroupConfiguration().Configure(modelBuilder.Entity<AccountAccountGroup>());
            //new ApplicationConfiguration().Configure(modelBuilder.Entity<Application>());
            //new TransactionConfiguration().Configure(modelBuilder.Entity<Transaction>());
            //new EntryConfiguration().Configure(modelBuilder.Entity<Entry>());

            base.OnModelCreating(modelBuilder);
        }

        private void SetTenantId(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entry>()
                .HasQueryFilter(mt => mt.TenantId == userService.TenantId);

            modelBuilder.Entity<Account>()
                .HasQueryFilter(mt => mt.TenantId == userService.TenantId);

            modelBuilder.Entity<TaxRate>()
                .HasQueryFilter(mt => mt.TenantId == userService.TenantId);

            //modelBuilder.Entity<Classification>()
            //    .HasQueryFilter(mt => mt.TenantId == _currentUser.TenantId);

            //modelBuilder.Entity<AccountAccountGroup>()
            //    .HasQueryFilter(mt => mt.TenantId == _currentUser.TenantId);

            //modelBuilder.Entity<Currency>()
            //    .HasQueryFilter(mt => mt.TenantId == _currentUser.TenantId);
        }
    }
}
