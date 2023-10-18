using Ubik.Accounting.Api.Models;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAccountGroup;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAllAccountGroups;

namespace Ubik.Accounting.Api.Features.AccountGroups.Mappers
{
    public static class AccountGroupMappers
    {
        public static IEnumerable<GetAllAccountGroupsResult> ToGetAllAccountGroupsResult(this IEnumerable<AccountGroup> accountGroups)
        {
            return accountGroups.Select(x => new GetAllAccountGroupsResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                ParentAccountGroupId = x.ParentAccountGroupId,
                Version = x.Version
            });
        }

        public static GetAccountGroupResult ToGetAccountGroupResult(this AccountGroup accountGroup)
        {
            return new GetAccountGroupResult()
            {
                Id = accountGroup.Id,
                Code = accountGroup.Code,
                Label = accountGroup.Label,
                Description = accountGroup.Description,
                ParentAccountGroupId = accountGroup.ParentAccountGroupId,
                Version = accountGroup.Version
            };
        }
    }
}
