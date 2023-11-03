using MassTransit;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Init
{
    internal static class AccountGroupsData
    {
        internal static void Load(AccountingContext context)
        {
            if (!context.AccountGroups.Any())
            {
                var baseValuesGeneral = new BaseValuesGeneral();
                var baseValuesForTenants = new BaseValuesForTenants();
                var baseValuesForUsers = new BaseValuesForUsers();
                var baseValuesForAccountGroups = new BaseValuesForAccountGroups();
                var baseValuesForAccountGroupClassifications = new BaseValuesForClassifications();
                var accountGroups = new AccountGroup[]
                {
                new AccountGroup
                {
                    Id = baseValuesForAccountGroups.AccountGroupIdFirstLvl1,
                    CreatedBy = baseValuesForUsers.UserId1,
                    CreatedAt = baseValuesGeneral.GenerationTime,
                    Code = "10",
                    Description = "Liquidités",
                    Label = "Liquidités",
                    ModifiedBy = baseValuesForUsers.UserId1,
                    ModifiedAt = baseValuesGeneral.GenerationTime,
                    AccountGroupClassificationId = baseValuesForAccountGroupClassifications.ClassificationId1,
                    ParentAccountGroupId = null,
                    Version = NewId.NextGuid(),
                    TenantId = baseValuesForTenants.TenantId
                },
                new AccountGroup
                {
                    Id = baseValuesForAccountGroups.AccountGroupId1,
                    CreatedBy = baseValuesForUsers.UserId1,
                    CreatedAt = baseValuesGeneral.GenerationTime,
                    Code = "102",
                    Description = "Liquidités bancaires",
                    Label = "Banques",
                    ModifiedBy = baseValuesForUsers.UserId1,
                    ModifiedAt = baseValuesGeneral.GenerationTime,
                    AccountGroupClassificationId = baseValuesForAccountGroupClassifications.ClassificationId1,
                    ParentAccountGroupId = baseValuesForAccountGroups.AccountGroupIdFirstLvl1,
                    Version =NewId.NextGuid(),
                    TenantId = baseValuesForTenants.TenantId
                },
                new AccountGroup
                {
                    Id = baseValuesForAccountGroups.AccountGroupId2,
                    CreatedBy = baseValuesForUsers.UserId1,
                    CreatedAt = baseValuesGeneral.GenerationTime,
                    Code = "103",
                    Description = "Liquidités autres",
                    Label = "Autres",
                    ModifiedBy = baseValuesForUsers.UserId1,
                    ModifiedAt = baseValuesGeneral.GenerationTime,
                    AccountGroupClassificationId = baseValuesForAccountGroupClassifications.ClassificationId2,
                    ParentAccountGroupId = baseValuesForAccountGroups.AccountGroupIdFirstLvl1,
                    Version = NewId.NextGuid(),
                    TenantId = baseValuesForTenants.TenantId
                },
                                new AccountGroup
                {
                    Id = baseValuesForAccountGroups.AccountGroupIdForDel,
                    CreatedBy = baseValuesForUsers.UserId1,
                    CreatedAt = baseValuesGeneral.GenerationTime,
                    Code = "104",
                    Description = "Autres actifs for removal",
                    Label = "To be removed Autres actifs",
                    ModifiedBy = baseValuesForUsers.UserId1,
                    ModifiedAt = baseValuesGeneral.GenerationTime,
                    AccountGroupClassificationId = baseValuesForAccountGroupClassifications.ClassificationId1,
                    ParentAccountGroupId = baseValuesForAccountGroups.AccountGroupIdFirstLvl1,
                    Version = NewId.NextGuid(),
                    TenantId = baseValuesForTenants.TenantId
                }

                            };

                foreach (AccountGroup ag in accountGroups)
                {
                    context.AccountGroups.Add(ag);
                }
                context.SaveChanges();
            }
        }
    }
}
