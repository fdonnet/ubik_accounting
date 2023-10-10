using Ubik.Accounting.Api.Dto;
using Ubik.Accounting.Api.Models;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAccount;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAllAccounts;

namespace Ubik.Accounting.Api.Features.Accounts.Mappers
{
    public static class AccountMappers
    {
        public static AddAccountResult ToAddAccountResult(this Account account)
        {
            return new AddAccountResult()
            {
                Id = account.Id,
                Code = account.Code,
                Label = account.Label,
                Description = account.Description,
                AccountGroupId = account.AccountGroupId,
                Version = account.Version
            };
        }

        public static GetAccountResult ToGetAccountResult(this Account account)
        {
            return new GetAccountResult()
            {
                Id = account.Id,
                Code = account.Code,
                Label = account.Label,
                Description = account.Description,
                AccountGroupId = account.AccountGroupId,
                Version = account.Version
            };
        }

        public static IEnumerable<GetAllAccountResult> ToGetAllAccountResult(this IEnumerable<Account> accounts)
        {
            return accounts.Select(x => new GetAllAccountResult() 
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                AccountGroupId = x.AccountGroupId,
                Version = x.Version
            });
        }

        public static Account ToAccount(this AddAccountCommand addAccountCommand)
        {
            return new Account()
            {
                Id = Guid.NewGuid(),
                Code = addAccountCommand.Code,
                Label = addAccountCommand.Label,
                Description = addAccountCommand.Description,
                AccountGroupId = addAccountCommand.AccountGroupId
            };
        }
    }
}
