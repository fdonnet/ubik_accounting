using MassTransit;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Init
{
    internal class AccountsAccountGroupsData
    {
        internal static void Load(AccountingContext context)
        {
            if (!context.AccountsAccountGroups.Any())
            {
                var baseValuesGeneral = new BaseValuesGeneral();
                var baseValuesForTenants = new BaseValuesForTenants();
                var baseValuesForUsers = new BaseValuesForUsers();
                var baseValuesForAccountGroups = new BaseValuesForAccountGroups();
                var baseValuesForAccounts = new BaseValuesForAccounts();

                var accountsAccountGroups = new AccountAccountGroup[]
                {
                new AccountAccountGroup
                {
                    Id = NewId.NextGuid(),
                    AccountGroupId = baseValuesForAccountGroups.AccountGroupId1,
                    AccountId= baseValuesForAccounts.AccountId1,
                    CreatedBy = baseValuesForUsers.UserId1,
                    CreatedAt = baseValuesGeneral.GenerationTime,
                    ModifiedBy = baseValuesForUsers.UserId1,
                    ModifiedAt = baseValuesGeneral.GenerationTime,
                    Version = NewId.NextGuid(),
                    TenantId = baseValuesForTenants.TenantId
                },
                new AccountAccountGroup
                {
                    Id = NewId.NextGuid(),
                    AccountGroupId = baseValuesForAccountGroups.AccountGroupId1,
                    AccountId= baseValuesForAccounts.AccountId2,
                    CreatedBy = baseValuesForUsers.UserId1,
                    CreatedAt = baseValuesGeneral.GenerationTime,
                    ModifiedBy = baseValuesForUsers.UserId1,
                    ModifiedAt = baseValuesGeneral.GenerationTime,
                    Version = NewId.NextGuid(),
                    TenantId = baseValuesForTenants.TenantId
                },
                                new AccountAccountGroup
                {
                    Id = NewId.NextGuid(),
                    AccountGroupId = baseValuesForAccountGroups.AccountGroupId2,
                    AccountId= baseValuesForAccounts.AccountId1,
                    CreatedBy = baseValuesForUsers.UserId1,
                    CreatedAt = baseValuesGeneral.GenerationTime,
                    ModifiedBy = baseValuesForUsers.UserId1,
                    ModifiedAt = baseValuesGeneral.GenerationTime,
                    Version = NewId.NextGuid(),
                    TenantId = baseValuesForTenants.TenantId
                }
            };
                foreach (AccountAccountGroup aag in accountsAccountGroups)
                {
                    context.AccountsAccountGroups.Add(aag);
                }
                context.SaveChanges();
            }
        }
    }
}
