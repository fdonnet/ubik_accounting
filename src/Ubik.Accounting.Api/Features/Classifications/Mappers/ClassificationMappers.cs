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
        public static GetClassificationStatusResult ToGetClassificationStatusResult(this ClassificationStatus current)
        {
            return new GetClassificationStatusResult()
            {
                Id = current.Id,
                IsReady = current.IsReady,
                MissingAccounts = current.MissingAccounts.ToGetClassificationAccountsMissingResult()
            };
        }

        public static IEnumerable<GetClassificationAccountsMissingResult> ToGetClassificationAccountsMissingResult(this IEnumerable<Account> current)
        {
            return current.Select(x => new GetClassificationAccountsMissingResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Category = x.Category,
                Domain = x.Domain,
                Description = x.Description,
                CurrencyId = x.CurrencyId,
                Version = x.Version
            });
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

        public static UpdateClassificationResult ToUpdateClassificationResult(this Classification classification)
        {
            return new UpdateClassificationResult()
            {
                Id = classification.Id,
                Code = classification.Code,
                Label = classification.Label,
                Description = classification.Description,
                Version = classification.Version
            };
        }

        public static DeleteClassificationResult ToDeleteClassificationResult(this IEnumerable<AccountGroup> accountGroups, Guid classificationId)
        {
            return new DeleteClassificationResult
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
