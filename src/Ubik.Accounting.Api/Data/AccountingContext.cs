using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
            new AccountConfiguration().Configure(modelBuilder.Entity<Account>());

            modelBuilder.Entity<AccountGroup>()
           .HasOne(s => s.ParentAccountGroup)
           .WithMany(m => m.ChildrenAccountGroups)
           .HasForeignKey(e => e.ParentAccountGroupId)
           .IsRequired(false);

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

            modelBuilder.Entity<AccountGroup>()
            .HasOne(a => a.CreatedByUser)
            .WithMany()
            .HasForeignKey(b => b.CreatedBy)
            .IsRequired(true);

            modelBuilder.Entity<AccountGroup>()
            .HasOne(a => a.ModifiedByUser)
            .WithMany()
            .HasForeignKey(b => b.ModifiedBy)
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

        public class AccountConfiguration : IEntityTypeConfiguration<Account>
        {
            public void Configure(EntityTypeBuilder<Account> builder)
            {
                builder.Property(a => a.Code)
                    .IsRequired()
                    .HasMaxLength(20);

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

                builder.HasIndex(a => a.Code)
                    .IsUnique();

                builder.HasIndex(a => a.TenantId);

                //TODO: Change that quick with userservice
                builder
                    .HasQueryFilter(a => a.TenantId == Guid.Parse("727449e8-e93c-49e6-a5e5-1bf145d3e62d"));

                //Relations
                builder
                    .HasOne(a => a.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(b => b.CreatedBy)
                    .IsRequired(true);

                builder
                    .HasOne(a => a.ModifiedByUser)
                    .WithMany()
                    .HasForeignKey(b => b.ModifiedBy)
                    .IsRequired(false);

                builder
                    .HasOne(g=>g.AccountGroup)
                    .WithMany(g=> g.Accounts)
                    .HasForeignKey(x=>x.AccountGroupId)
                    .IsRequired(true);
            }
        }
    }
}
