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

        public static AccountGroupStandardResult ToAccountGroupStandardResult(this AccountGroup accountGroup)
        {
            return new AccountGroupStandardResult()
            {
                Id = accountGroup.Id,
                Code = accountGroup.Code,
                Label = accountGroup.Label,
                Description = accountGroup.Description,
                ParentAccountGroupId = accountGroup.ParentAccountGroupId,
                AccountGroupClassificationId = accountGroup.ClassificationId,
                Version = accountGroup.Version
            };
        }
    }
}
