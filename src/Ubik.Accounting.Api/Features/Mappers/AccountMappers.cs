using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.Api.Features.Mappers
{
    public static class AccountMappers
    {
        public static IEnumerable<AccountStandardResult> ToAccountStandardResults(this IEnumerable<Account> accounts)
        {
            return accounts.Select(x => new AccountStandardResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                Version = x.Version,
                CurrencyId = x.CurrencyId
            });
        }

        public static AccountStandardResult ToAccountStandardResult(this Account account)
        {
            return new AccountStandardResult()
            {
                Id = account.Id,
                Code = account.Code,
                Label = account.Label,
                Category = account.Category,
                Domain = account.Domain,
                Description = account.Description,
                CurrencyId = account.CurrencyId,
                Version = account.Version
            };
        }

        public static AccountGroupLinkResult ToAccountGroupLinkResult(this AccountAccountGroup current)
        {
            return new AccountGroupLinkResult()
            {
                Id = current.Id,
                AccountGroupId = current.AccountGroupId,
                AccountId = current.AccountId,
                Version = current.Version
            };
        }

        public static IEnumerable<AccountGroupLinkResult> ToAccountGroupLinkResults(this IEnumerable<AccountAccountGroup> current)
        {
            return current.Select(x => new AccountGroupLinkResult()
            {
                Id = x.Id,
                AccountGroupId = x.AccountGroupId,
                AccountId = x.AccountId,
                Version = x.Version
            });
        }
    }
}
