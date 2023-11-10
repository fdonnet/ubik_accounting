using MassTransit;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Init
{
    internal static class ClassificationsData
    {
        internal static void Load(AccountingContext context)
        {
            if (!context.Classifications.Any())
            {
                var baseValuesGeneral = new BaseValuesGeneral();
                var baseValuesForTenants = new BaseValuesForTenants();
                var baseValuesForUsers = new BaseValuesForUsers();
                var baseValuesForAccountGroupClassifications = new BaseValuesForClassifications();
                var accountGroupClassifications = new Classification[]
                {
                    new Classification
                    {
                    Id = baseValuesForAccountGroupClassifications.ClassificationId1,
                    CreatedBy = baseValuesForUsers.UserId1,
                    CreatedAt = baseValuesGeneral.GenerationTime,
                    Code = "SWISSPLAN",
                    Description = "Plan comptable suisse",
                    Label = "Test",
                    ModifiedBy = baseValuesForUsers.UserId1,
                    ModifiedAt = baseValuesGeneral.GenerationTime,
                    Version = NewId.NextGuid(),
                    TenantId = baseValuesForTenants.TenantId
                    },
                    new Classification
                    {
                    Id = baseValuesForAccountGroupClassifications.ClassificationId2,
                    CreatedBy = baseValuesForUsers.UserId1,
                    CreatedAt = baseValuesGeneral.GenerationTime,
                    Code = "SWISSPLAN2",
                    Description = "Plan comptable suisse2",
                    Label = "Test",
                    ModifiedBy = baseValuesForUsers.UserId1,
                    ModifiedAt = baseValuesGeneral.GenerationTime,
                    Version = NewId.NextGuid(),
                    TenantId = baseValuesForTenants.TenantId
                    }
                };

                foreach (Classification cl in accountGroupClassifications)
                {
                    context.Classifications.Add(cl);
                }
                context.SaveChanges();
            }
        }
    }
}
