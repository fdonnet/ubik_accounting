using MassTransit;
using Ubik.Accounting.Api.Features.Classifications.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Events;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.Accounting.Contracts.Classifications.Commands;
using Ubik.Accounting.Contracts.Classifications.Events;
using Ubik.Accounting.Contracts.Classifications.Results;

namespace Ubik.Accounting.Api.Features.Classifications.Mappers
{
    public static class ClassificationMappers
    {
        public static ClassificationStandardResult ToAddClassificationResult(this Classification classification)
        {
            return new ClassificationStandardResult()
            {
                Id = classification.Id,
                Code = classification.Code,
                Label = classification.Label,
                Description = classification.Description,
                Version = classification.Version
            };
        }

        public static ClassificationDeleteResult ToDeleteClassificationResult(this IEnumerable<AccountGroup> accountGroups, Guid classificationId)
        {
            return new ClassificationDeleteResult
            {
                Id = classificationId,
                DeletedAccountGroups = accountGroups.Select(x => new AccountGroupStandardResult()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Label = x.Label,
                    ParentAccountGroupId = x.ParentAccountGroupId,
                })
            };
        }

        public static ClassificationDeleted ToClassificationDeleted(this IEnumerable<AccountGroup> accountGroups, Guid classificationId)
        {
            return new ClassificationDeleted
            {
                Id = classificationId,
                AccountGroupsDeleted = accountGroups.Select(x => new AccountGroupDeleted()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Label = x.Label,
                    ParentAccountGroupId = x.ParentAccountGroupId,
                })
            };
        }

    }
}
