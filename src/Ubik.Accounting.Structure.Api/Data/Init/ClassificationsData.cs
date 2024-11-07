using MassTransit;
using System.Security.Cryptography.Xml;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.DB.Common.Models;

namespace Ubik.Accounting.Structure.Api.Data.Init
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
                    Code = "SWISSPLAN-FULL",
                    Description = "For testing purposes",
                    Label = "Standard Swiss Plan (full)",
                    AuditInfo = new AuditData(baseValuesGeneral.GenerationTime, baseValuesForUsers.UserId1
                        ,baseValuesGeneral.GenerationTime, baseValuesForUsers.UserId1),
                    Version = NewId.NextGuid(),
                    TenantId = baseValuesForTenants.TenantId
                    },
                    new Classification
                    {
                    Id = baseValuesForAccountGroupClassifications.ClassificationId2,
                    AuditInfo = new AuditData(baseValuesGeneral.GenerationTime, baseValuesForUsers.UserId1
                        ,baseValuesGeneral.GenerationTime, baseValuesForUsers.UserId1),
                    Code = "SWISSPLAN-TEST1",
                    Description = null,
                    Label = "Test data",
                    Version = baseValuesForAccountGroupClassifications.ClassificationId2,
                    TenantId = baseValuesForTenants.TenantId
                    },
                    new Classification
                    {
                    Id = baseValuesForAccountGroupClassifications.ClassificationId3,
                    AuditInfo = new AuditData(baseValuesGeneral.GenerationTime, baseValuesForUsers.UserId1
                        ,baseValuesGeneral.GenerationTime, baseValuesForUsers.UserId1),
                    Code = "SWISSPLAN-TEST2",
                    Description = null,
                    Label = "Test data",
                    Version = baseValuesForAccountGroupClassifications.ClassificationId3,
                    TenantId = baseValuesForTenants.TenantId
                    },
                    new Classification
                    {
                    Id = baseValuesForAccountGroupClassifications.ClassificationIdForDel,
                    AuditInfo = new AuditData(baseValuesGeneral.GenerationTime, baseValuesForUsers.UserId1
                        ,baseValuesGeneral.GenerationTime, baseValuesForUsers.UserId1),
                    Code = "SWISSPLAN-TESTZZZZ",
                    Description = null,
                    Label = "Test data",
                    Version = baseValuesForAccountGroupClassifications.ClassificationIdForDel,
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
