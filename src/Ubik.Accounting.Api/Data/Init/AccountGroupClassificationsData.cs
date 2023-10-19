using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Init
{
    internal static class AccountGroupClassificationsData
    {
        internal static void Load(AccountingContext context)
        {
            if (!context.AccountGroupClassifications.Any())
            {
                var baseValuesGeneral = new BaseValuesGeneral();
                var baseValuesForTenants = new BaseValuesForTenants();
                var baseValuesForUsers = new BaseValuesForUsers();
                var baseValuesForAccountGroupClassifications = new BaseValuesForAccountGroupClassifications();
                var accountGroupClassifications = new AccountGroupClassification[]
                {
                    new AccountGroupClassification
                    {
                    Id = baseValuesForAccountGroupClassifications.AccountGroupClassificationId1,
                    CreatedBy = baseValuesForUsers.UserId1,
                    CreatedAt = baseValuesGeneral.GenerationTime,
                    Code = "SWISSPLAN",
                    Description = "Plan comptable suisse",
                    Label = "Test",
                    ModifiedBy = baseValuesForUsers.UserId1,
                    ModifiedAt = baseValuesGeneral.GenerationTime,
                    Version = Guid.NewGuid(),
                    TenantId = baseValuesForTenants.TenantId
                    },
                    new AccountGroupClassification
                    {
                    Id = baseValuesForAccountGroupClassifications.AccountGroupClassificationId2,
                    CreatedBy = baseValuesForUsers.UserId1,
                    CreatedAt = baseValuesGeneral.GenerationTime,
                    Code = "SWISSPLAN2",
                    Description = "Plan comptable suisse2",
                    Label = "Test",
                    ModifiedBy = baseValuesForUsers.UserId1,
                    ModifiedAt = baseValuesGeneral.GenerationTime,
                    Version = Guid.NewGuid(),
                    TenantId = baseValuesForTenants.TenantId
                    }
                };

                foreach (AccountGroupClassification cl in accountGroupClassifications)
                {
                    context.AccountGroupClassifications.Add(cl);
                }
                context.SaveChanges();
            }
        }
    }
}
