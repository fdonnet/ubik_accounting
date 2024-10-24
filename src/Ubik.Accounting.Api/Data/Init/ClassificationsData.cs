using MassTransit;
using Ubik.Accounting.Api.Models;

namespace Ubik.Accounting.Api.Data.Init
{
    internal static class ClassificationsData
    {
        internal static void Load(AccountingDbContext context)
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
                    Code = "SWISSPLAN-FULL",
                    Description = "For testing purposes",
                    Label = "Standard Swiss Plan (full)",
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
                    Code = "SWISSPLAN-TEST1",
                    Description = null,
                    Label = "Test data",
                    ModifiedBy = baseValuesForUsers.UserId1,
                    ModifiedAt = baseValuesGeneral.GenerationTime,
                    Version = baseValuesForAccountGroupClassifications.ClassificationId2,
                    TenantId = baseValuesForTenants.TenantId
                    },
                    new Classification
                    {
                    Id = baseValuesForAccountGroupClassifications.ClassificationId3,
                    CreatedBy = baseValuesForUsers.UserId1,
                    CreatedAt = baseValuesGeneral.GenerationTime,
                    Code = "SWISSPLAN-TEST2",
                    Description = null,
                    Label = "Test data",
                    ModifiedBy = baseValuesForUsers.UserId1,
                    ModifiedAt = baseValuesGeneral.GenerationTime,
                    Version = baseValuesForAccountGroupClassifications.ClassificationId3,
                    TenantId = baseValuesForTenants.TenantId
                    },
                    new Classification
                    {
                    Id = baseValuesForAccountGroupClassifications.ClassificationIdForDel,
                    CreatedBy = baseValuesForUsers.UserId1,
                    CreatedAt = baseValuesGeneral.GenerationTime,
                    Code = "SWISSPLAN-TESTZZZZ",
                    Description = null,
                    Label = "Test data",
                    ModifiedBy = baseValuesForUsers.UserId1,
                    ModifiedAt = baseValuesGeneral.GenerationTime,
                    Version = baseValuesForAccountGroupClassifications.ClassificationId3,
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
