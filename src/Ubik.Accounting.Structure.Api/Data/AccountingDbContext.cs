using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Structure.Api.Data.Config;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Services;
using Ubik.DB.Common.Extensions;

namespace Ubik.Accounting.Structure.Api.Data
{
    public class AccountingDbContext(DbContextOptions<AccountingDbContext> options
        , ICurrentUser userService) : DbContext(options)
    {
        private readonly ICurrentUser _currentUser = userService;

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountGroup> AccountGroups { get; set; }
        public DbSet<AccountAccountGroup> AccountsAccountGroups { get; set; }
        public DbSet<Classification> Classifications { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<VatRate> VatRates { get; set; }
        public DbSet<AccountVatConfig> AccountVatConfigs { get; set; }


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
            ChangeTracker.SetSpecialFields(_currentUser);
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
            new CurrencyConfiguration().Configure(modelBuilder.Entity<Currency>());
            new ClassificationConfiguration().Configure(modelBuilder.Entity<Classification>());
            new AccountGroupConfiguration().Configure(modelBuilder.Entity<AccountGroup>());
            new AccountConfiguration().Configure(modelBuilder.Entity<Account>());
            new AccountAccountGroupConfiguration().Configure(modelBuilder.Entity<AccountAccountGroup>());
            new TransactionConfiguration().Configure(modelBuilder.Entity<Transaction>());
            new EntryConfiguration().Configure(modelBuilder.Entity<Entry>());
            new VatRateConfiguration().Configure(modelBuilder.Entity<VatRate>());
            new AccountVatConfigConfiguration().Configure(modelBuilder.Entity<AccountVatConfig>());

            base.OnModelCreating(modelBuilder);
        }

        private void SetTenantId(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasQueryFilter(mt => mt.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<AccountGroup>()
                .HasQueryFilter(mt => mt.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<Classification>()
                .HasQueryFilter(mt => mt.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<AccountAccountGroup>()
                .HasQueryFilter(mt => mt.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<Currency>()
                .HasQueryFilter(mt => mt.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<Transaction>()
                .HasQueryFilter(mt => mt.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<Entry>()
                .HasQueryFilter(mt => mt.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<VatRate>()
               .HasQueryFilter(mt => mt.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<AccountVatConfig>()
               .HasQueryFilter(mt => mt.TenantId == _currentUser.TenantId);
        }
    }
}
