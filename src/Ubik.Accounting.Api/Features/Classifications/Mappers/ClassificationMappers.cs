using MassTransit;
using Ubik.Accounting.Api.Features.Classifications.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Contracts.AccountGroups.Events;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.Accounting.Contracts.Classifications.Commands;
using Ubik.Accounting.Contracts.Classifications.Events;
using Ubik.Accounting.Contracts.Classifications.Results;

namespace Ubik.Accounting.Api.Features.Classifications.Mappers
{
    public static class ClassificationMappers
    {
        public static IEnumerable<GetAllClassificationsResult> ToGetAllClassificationsResult(this IEnumerable<Classification> current)
        {
            return current.Select(x => new GetAllClassificationsResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                Version = x.Version
            });
        }

        public static GetClassificationResult ToGetClassificationResult(this Classification current)
        {
            return new GetClassificationResult()
            {
                Id = current.Id,
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version
            };
        }

        public static GetClassificationStatusResult ToGetClassificationStatusResult(this ClassificationStatus current)
        {
            return new GetClassificationStatusResult()
            {
                Id = current.Id,
                IsReady = current.IsReady,
                MissingAccounts = current.MissingAccounts.ToGetClassificationAccountsMissingResult()
            };
        }

        public static IEnumerable<GetClassificationAccountsResult> ToGetClassificationAccountsResult(this IEnumerable<Account> current)
        {
            return current.Select(x => new GetClassificationAccountsResult()
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
        public static AddClassificationResult ToAddClassificationResult(this Classification classification)
        {
            return new AddClassificationResult()
            {
                Id = classification.Id,
                Code = classification.Code,
                Label = classification.Label,
                Description = classification.Description,
                Version = classification.Version
            };
        }

    }
}
