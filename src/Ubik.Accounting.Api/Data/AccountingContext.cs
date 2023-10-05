using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data
{
    public class AccountingContext : DbContext
    {
        public AccountingContext(DbContextOptions<AccountingContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountGroup> AccountGroups { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<TaxRate> TaxRates { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            modelBuilder.Entity<Account>()
            .HasOne(a => a.CreatedByUser)
            .WithMany()
            .HasForeignKey(b => b.CreatedBy)
            .IsRequired(true);

            modelBuilder.Entity<Account>()
            .HasOne(a => a.ModifiedByUser)
            .WithMany()
            .HasForeignKey(b => b.ModifiedBy)
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
    }
}
