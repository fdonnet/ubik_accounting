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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountGroup>()
           .HasOne(s => s.ParentAccountGroup)
           .WithMany(m => m.ChildrenAccountGroups)
           .HasForeignKey(e => e.ParentAccountGroupId)
           .IsRequired(false);

            modelBuilder.Entity<MainEntry>()
           .HasMany(s => s.CounterpartyEntries)
           .WithOne(m => m.MainEntry)
           .HasForeignKey(e => e.MainEntryId)
           .IsRequired(true);

            modelBuilder.Entity<MainEntry>()
            .HasOne(s => s.Currency)
            .WithMany()
            .HasForeignKey(e => e.CurrencyId)
            .IsRequired(true);

            modelBuilder.Entity<MainEntry>()
            .HasOne(s => s.OriginalCurrency)
            .WithMany()
            .HasForeignKey(e => e.OriginalCurrencyId)
            .IsRequired(false);

            modelBuilder.Entity<CounterpartyEntry>()
            .HasOne(s => s.OriginalCurrency)
            .WithMany()
            .HasForeignKey(e => e.OriginalCurrencyId)
            .IsRequired(false);
        }
    }
}
