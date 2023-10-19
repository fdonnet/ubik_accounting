using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data.Config;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Services;
using Ubik.DB.Common.Extensions;

namespace Ubik.Accounting.Api.Data
{
    public class AccountingContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        public AccountingContext(DbContextOptions<AccountingContext> options, ICurrentUserService userService)
            : base(options)
        {
            _currentUserService = userService;
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountGroup> AccountGroups { get; set; }
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
            ChangeTracker.SetSpecialFields(_currentUserService);
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new AccountGroupClassificationConfiguration().Configure(modelBuilder.Entity<AccountGroupClassification>());
            new AccountGroupConfiguration().Configure(modelBuilder.Entity<AccountGroup>());
            new AccountConfiguration().Configure(modelBuilder.Entity<Account>());

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
    }
}
