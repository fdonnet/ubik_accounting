using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using Ubik.Accounting.Api.Data.Config;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.Common.Services;
using Ubik.DB.Common.Extensions;

namespace Ubik.Accounting.Api.Data
{
    public class AccountingContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly Guid _tenantId;
        public AccountingContext(DbContextOptions<AccountingContext> options, ICurrentUserService userService)
            : base(options)
        {
            _currentUserService = userService;
            _tenantId = userService.CurrentUser.TenantIds[0];
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountGroup> AccountGroups { get; set; }
        public DbSet<AccountAccountGroup> AccountsAccountGroups { get; set; }
        public DbSet<AccountGroupClassification> AccountGroupClassifications { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<TaxRate> TaxRates { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
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
                    CustomErrors = new List<CustomError> { err }
                };

                throw conflict;
            }
        }

        public void SetAuditAndSpecialFields()
        {
            ChangeTracker.SetSpecialFields(_currentUserService);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Build for Masstransit outbox
            modelBuilder.AddInboxStateEntity(); 
            modelBuilder.AddOutboxMessageEntity(); 
            modelBuilder.AddOutboxStateEntity();

            //TenantId
            SetTenantId(modelBuilder);

            new AccountGroupClassificationConfiguration().Configure(modelBuilder.Entity<AccountGroupClassification>());
            new AccountGroupConfiguration().Configure(modelBuilder.Entity<AccountGroup>());
            new AccountConfiguration().Configure(modelBuilder.Entity<Account>());
            new AccountAccountGroupConfiguration().Configure(modelBuilder.Entity<AccountAccountGroup>());

            modelBuilder.Entity<Entry>()
            .HasOne(s => s.MainEntry)
            .WithMany(e => e.CounterpartyEntries)
            .HasForeignKey(e => e.MainEntryId)
            .IsRequired(false);

            modelBuilder.Entity<Entry>()
            .HasOne(s => s.OriginalCurrency)
            .WithMany()
            .HasForeignKey(e => e.OriginalCurrencyId)
            .IsRequired(false);

            modelBuilder.Entity<Entry>()
            .HasOne(s => s.TaxRate)
            .WithMany()
            .HasForeignKey(e => e.TaxRateId)
            .IsRequired(false);

            modelBuilder.Entity<Currency>()
            .HasOne(a => a.CreatedByUser)
            .WithMany()
            .HasForeignKey(b => b.CreatedBy)
            .IsRequired(true);

            modelBuilder.Entity<Currency>()
            .HasOne(a => a.ModifiedByUser)
            .WithMany()
            .HasForeignKey(b => b.ModifiedBy)
            .IsRequired(false);

            modelBuilder.Entity<Entry>()
            .HasOne(a => a.CreatedByUser)
            .WithMany()
            .HasForeignKey(b => b.CreatedBy)
            .IsRequired(true);

            modelBuilder.Entity<Entry>()
            .HasOne(a => a.ModifiedByUser)
            .WithMany()
            .HasForeignKey(b => b.ModifiedBy)
            .IsRequired(false);

            modelBuilder.Entity<TaxRate>()
            .HasOne(a => a.CreatedByUser)
            .WithMany()
            .HasForeignKey(b => b.CreatedBy)
            .IsRequired(true);

            modelBuilder.Entity<TaxRate>()
            .HasOne(a => a.ModifiedByUser)
            .WithMany()
            .HasForeignKey(b => b.ModifiedBy)
            .IsRequired(false);
        }

        private void SetTenantId(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasQueryFilter(mt => mt.TenantId == _tenantId);

            modelBuilder.Entity<AccountGroup>()
                .HasQueryFilter(mt => mt.TenantId == _tenantId);

            modelBuilder.Entity<AccountGroupClassification>()
                .HasQueryFilter(mt => mt.TenantId == _tenantId);

            modelBuilder.Entity<AccountAccountGroup>()
                .HasQueryFilter(mt => mt.TenantId == _tenantId);
        }
    }
}
