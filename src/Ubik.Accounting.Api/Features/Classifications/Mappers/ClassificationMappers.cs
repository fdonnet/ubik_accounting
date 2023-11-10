using Ubik.Accounting.Api.Features.Classifications.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
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
    }
}
