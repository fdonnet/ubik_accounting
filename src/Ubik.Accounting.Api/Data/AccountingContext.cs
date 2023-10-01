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
        }
    }
}
