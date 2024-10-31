using MassTransit;
using Ubik.Accounting.Structure.Api.Features.Classifications.CustomPoco;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.Accounting.Structure.Contracts.AccountGroups.Events;
using Ubik.Accounting.Structure.Contracts.AccountGroups.Results;
using Ubik.Accounting.Structure.Contracts.Classifications.Commands;
using Ubik.Accounting.Structure.Contracts.Classifications.Events;
using Ubik.Accounting.Structure.Contracts.Classifications.Results;

namespace Ubik.Accounting.Structure.Api.Mappers
{
    public static class ClassificationMappers
    {
        public static IEnumerable<ClassificationStandardResult> ToClassificationStandardResults(this IEnumerable<Classification> current)
        {
            return current.Select(x => new ClassificationStandardResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                Version = x.Version
            });
        }

        public static ClassificationStandardResult ToClassificationStandardResult(this Classification current)
        {
            return new ClassificationStandardResult()
            {
                Id = current.Id,
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version
            };
        }

        public static ClassificationStatusResult ToClassificationStatusResult(this ClassificationStatus current)
        {
            return new ClassificationStatusResult()
            {
                Id = current.Id,
                IsReady = current.IsReady,
                MissingAccounts = current.MissingAccounts.ToAccountStandardResults()
            };
        }

        public static Classification ToClassification(this AddClassificationCommand addClassificationCommand)
        {
            return new Classification()
            {
                Id = NewId.NextGuid(),
                Code = addClassificationCommand.Code,
                Label = addClassificationCommand.Label,
                Description = addClassificationCommand.Description
            };
        }

        public static Classification ToClassification(this Classification forUpd, Classification classification)
        {
            classification.Id = forUpd.Id;
            classification.Code = forUpd.Code;
            classification.Label = forUpd.Label;
            classification.Description = forUpd.Description;
            classification.Version = forUpd.Version;

            return classification;
        }
        public static ClassificationAdded ToClassificationAdded(this Classification classification)
        {
            return new ClassificationAdded
            {
                Id = classification.Id,
                Code = classification.Code,
                Label = classification.Label,
                Description = classification.Description,
                Version = classification.Version
            };
        }

        public static ClassificationUpdated ToClassificationUpdated(this Classification classification)
        {
            return new ClassificationUpdated
            {
                Id = classification.Id,
                Code = classification.Code,
                Label = classification.Label,
                Description = classification.Description,
                Version = classification.Version
            };
        }

        public static Classification ToClassification(this UpdateClassificationCommand updateClassificationCommand)
        {
            return new Classification()
            {
                Id = updateClassificationCommand.Id,
                Code = updateClassificationCommand.Code,
                Label = updateClassificationCommand.Label,
                Description = updateClassificationCommand.Description,
                Version = updateClassificationCommand.Version
            };
        }

        public static ClassificationDeleted ToClassificationDeleted(this IEnumerable<AccountGroup> accountGroups, Guid classificationId)
        {
            return new ClassificationDeleted
            {
                Id = classificationId,
                AccountGroupsDeleted = accountGroups.Select(x => new AccountGroupStandardResult()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Label = x.Label,
                    ParentAccountGroupId = x.ParentAccountGroupId,
                })
            };
        }

        public static ClassificationDeleteResult ToClassificationDeleteResult(this IEnumerable<AccountGroup> accountGroups, Guid classificationId)
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
    }
}
