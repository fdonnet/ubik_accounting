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

            modelBuilder.Entity<Entry>()
            .HasOne(s => s.DebitAccount)
            .WithMany()
            .HasForeignKey(e => e.DebitAccountId)
            .IsRequired(true);

            modelBuilder.Entity<Entry>()
            .HasOne(s => s.CreditAccount)
            .WithMany()
            .HasForeignKey(e => e.CreditAccountId)
            .IsRequired(true);
        }
    }
}
