using Ubik.Accounting.Api.Models;
using static Ubik.Accounting.Api.Features.Accounts.Commands.AddAccount;
using static Ubik.Accounting.Api.Features.Accounts.Commands.UpdateAccount;
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
                Version = account.Version
            };
        }

        public static UpdateAccountResult ToUpdateAccountResult(this Account account)
        {
            return new UpdateAccountResult()
            {
                Id = account.Id,
                Code = account.Code,
                Label = account.Label,
                Description = account.Description,
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
                Version = account.Version
            };
        }

        public static IEnumerable<GetAllAccountsResult> ToGetAllAccountResult(this IEnumerable<Account> accounts)
        {
            return accounts.Select(x => new GetAllAccountsResult() 
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
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
            };
        }

        public static Account ToAccount(this UpdateAccountCommand updateAccountCommand, Account account)
        {
            account.Id = updateAccountCommand.Id;
            account.Code = updateAccountCommand.Code;
            account.Label = updateAccountCommand.Label;
            account.Description = updateAccountCommand.Description;
            account.Version = updateAccountCommand.Version;
            return account;
        }
    }
}
