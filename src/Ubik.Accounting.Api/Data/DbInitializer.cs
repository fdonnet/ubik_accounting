using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AccountingContext context)
        {
            var tenantId = Guid.Parse("727449e8-e93c-49e6-a5e5-1bf145d3e62d");

            var userId1 = Guid.Parse("9124f11f-20dd-4888-88f8-428e59bbc53e");
            var userId2 = Guid.NewGuid();

            var accountGroupId1 = Guid.Parse("1524f11f-20dd-4888-88f8-428e59bbc22a");

            var now = DateTime.UtcNow;

            if (!context.Users.Any())
            {
                var users = new User[]
                            {
                new User
                {
                    Id = userId1,
                    Name = "testuser",
                    Email = "test@gmail.com",
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
                        Label = "Banque 1",
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
