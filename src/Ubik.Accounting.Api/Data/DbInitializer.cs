using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Api.Service;

namespace Ubik.Accounting.Api.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AccountingContext context)
        {
            var tenantId = Guid.NewGuid();

            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            var accountGroupId1 = Guid.NewGuid();
            var accountGroupId2 = Guid.NewGuid();

            var now = DateTime.UtcNow;

            if (!context.Users.Any())
            {
                var users = new User[]
                            {
                new User
                {
                    Id = userId1,
                    Name = "fdonnet",
                    Email = "donnetf@gmail.com",
                    TenantId = tenantId
                },
                new User
                {
                    Id = userId2,
                    Name = "test01",
                    Email = "test01@gmail.com",
                    TenantId = tenantId
                },
                            };

                foreach (User u in users)
                {
                    context.Users.Add(u);
                }
                context.SaveChanges();
            }

            if (!context.AccountGroups.Any())
            {
                var accountGroups = new AccountGroup[]
                            {
                new AccountGroup
                {
                    Id = accountGroupId1,
                    CreatedBy = userId1,
                    CreatedAt = now,
                    Code = "102",
                    Description = "Liquidités bancaires",
                    Label = "Banques",
                    ModifiedBy = userId1,
                    ModifiedAt = now,
                    ParentAccountGroupId = null,
                    Version = Guid.NewGuid(),
                    TenantId = tenantId
                }
                            };

                foreach (AccountGroup ag in accountGroups)
                {
                    context.AccountGroups.Add(ag);
                }
                context.SaveChanges();
            }

            if (!context.Accounts.Any())
            {
                var accounts = new Account[]
                {
                    new Account
                    {
                        Id= Guid.NewGuid(),
                        AccountGroupId = accountGroupId1,
                        Code = "1020",
                        CreatedBy= userId1,
                        CreatedAt = now,
                        Label = "UBS xxx Cash (CHF)",
                        Description = "Compte bancaire cash",
                        ModifiedBy= userId1,
                        ModifiedAt = now,
                        TenantId= tenantId,
                        Version = Guid.NewGuid()
                    }
                };

                foreach (Account a in accounts)
                {
                    context.Accounts.Add(a);
                }
                context.SaveChanges();
            }

        }
    }
}
