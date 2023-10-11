using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data
{
    public class DbInitializer
    {
        public Guid TenantId { get; } = Guid.Parse("727449e8-e93c-49e6-a5e5-1bf145d3e62d");
        
        public Guid AccountId1 { get; } = Guid.Parse("7777f11f-20dd-4888-88f8-428e59bbc537");
        public string AccountCode1 { get; } = "1020";

        public Guid UserId1 { get; } = Guid.Parse("9124f11f-20dd-4888-88f8-428e59bbc53e");
        
        public Guid AccountGroupId1 { get; } = Guid.Parse("1524f11f-20dd-4888-88f8-428e59bbc22a");

        public void Initialize(AccountingContext context)
        {
            var userId2 = Guid.NewGuid();
            var now = DateTime.UtcNow;

            if (!context.Users.Any())
            {
                var users = new User[]
                            {
                new User
                {
                    Id = UserId1,
                    Name = "testuser",
                    Email = "test@gmail.com",
                    TenantId = TenantId
                },
                new User
                {
                    Id = userId2,
                    Name = "test01",
                    Email = "test01@gmail.com",
                    TenantId = TenantId
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
                    Id = AccountGroupId1,
                    CreatedBy = UserId1,
                    CreatedAt = now,
                    Code = "102",
                    Description = "Liquidités bancaires",
                    Label = "Banques",
                    ModifiedBy = UserId1,
                    ModifiedAt = now,
                    ParentAccountGroupId = null,
                    Version = Guid.NewGuid(),
                    TenantId = TenantId
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
                        Id= AccountId1,
                        AccountGroupId = AccountGroupId1,
                        Code = AccountCode1,
                        CreatedBy= UserId1,
                        CreatedAt = now,
                        Label = "Banque 1",
                        Description = "Compte bancaire cash",
                        ModifiedBy= UserId1,
                        ModifiedAt = now,
                        TenantId= TenantId,
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
