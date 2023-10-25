using Ubik.Accounting.Api.Models;
using Ubik.ApiService.DB.Enums;

namespace Ubik.Accounting.Api.Data.Init
{
    internal static class AccountsData
    {
        internal static void Load(AccountingContext context)
        {
            if (!context.Accounts.Any())
            {
                var baseValuesGeneral = new BaseValuesGeneral();
                var baseValuesForTenants = new BaseValuesForTenants();
                var baseValuesForUsers = new BaseValuesForUsers();
                var baseValuesForAccounts = new BaseValuesForAccounts();
                var baseValuesForCurrencies = new BaseValuesForCurrencies();

                var accounts = new Account[]
                {
                    new Account
                    {
                        Id= baseValuesForAccounts.AccountId1,
                        Code = baseValuesForAccounts.AccountCode1,
                        CurrencyId = baseValuesForCurrencies.CurrencyId1,
                        CreatedBy= baseValuesForUsers.UserId1,
                        CreatedAt = baseValuesGeneral.GenerationTime,
                        Label = "Banque 1",
                        Description = "Compte bancaire cash",
                        Category = AccountCategory.Liquidity,
                        Domain = AccountDomain.Asset,
                        ModifiedBy= baseValuesForUsers.UserId1,
                        ModifiedAt = baseValuesGeneral.GenerationTime,
                        TenantId= baseValuesForTenants.TenantId,
                        Version = Guid.NewGuid()
                    },
                    new Account
                    {
                        Id= baseValuesForAccounts.AccountId2,
                        Code = "1030",
                        CurrencyId = baseValuesForCurrencies.CurrencyId2,
                        CreatedBy= baseValuesForUsers.UserId1,
                        CreatedAt = baseValuesGeneral.GenerationTime,
                        Label = "Banque 2",
                        Description = "Compte bancaire cash",
                        Category = AccountCategory.Liquidity,
                        Domain = AccountDomain.Asset,
                        ModifiedBy= baseValuesForUsers.UserId1,
                        ModifiedAt = baseValuesGeneral.GenerationTime,
                        TenantId= baseValuesForTenants.TenantId,
                        Version = Guid.NewGuid()
                    },

                    new Account
                    {
                        Id= baseValuesForAccounts.AccountIdForDel,
                        Code = "2030",
                        CurrencyId = baseValuesForCurrencies.CurrencyId1,
                        CreatedBy= baseValuesForUsers.UserId1,
                        CreatedAt = baseValuesGeneral.GenerationTime,
                        Label = "Banque for removal",
                        Description = "Compte bancaire cash old",
                        Category = AccountCategory.Liquidity,
                        Domain = AccountDomain.Asset,
                        ModifiedBy= baseValuesForUsers.UserId1,
                        ModifiedAt = baseValuesGeneral.GenerationTime,
                        TenantId= baseValuesForTenants.TenantId,
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
