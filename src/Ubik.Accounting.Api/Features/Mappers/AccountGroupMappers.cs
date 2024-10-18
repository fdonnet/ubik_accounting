using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Results;

namespace Ubik.Accounting.Api.Features.Mappers
{
    public static class AccountGroupMappers
    {
        public static IEnumerable<AccountGroupStandardResult> ToAccountGroupStandardResults(this IEnumerable<AccountGroup> accountGroups)
        {
            return accountGroups.Select(x => new AccountGroupStandardResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                ParentAccountGroupId = x.ParentAccountGroupId,
                AccountGroupClassificationId = x.ClassificationId,
                Version = x.Version
            });
        }
    }
}
