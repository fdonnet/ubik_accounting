using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Accounts.Events;
using Ubik.Accounting.Contracts.Accounts.Results;
using static Ubik.Accounting.Api.Features.Accounts.Commands.UpdateAccount;
using static Ubik.Accounting.Api.Features.Accounts.Queries.GetAccount;

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
                Category = account.Category,
                Domain = account.Domain,
                Description = account.Description,
                CurrencyId = account.CurrencyId,
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
                Category = account.Category,
                Domain = account.Domain,
                Description = account.Description,
                CurrencyId = account.CurrencyId,
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
                Category = account.Category,
                Domain = account.Domain,
                Description = account.Description,
                CurrencyId = account.CurrencyId,
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
                Category = x.Category,
                Domain = x.Domain,
                Description = x.Description,
                CurrencyId = x.CurrencyId,
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
                Category = addAccountCommand.Category,
                Domain = addAccountCommand.Domain,
                Description = addAccountCommand.Description,
                CurrencyId = addAccountCommand.CurrencyId
            };
        }

        public static Account ToAccount(this UpdateAccountCommand updateAccountCommand, Account account)
        {
            account.Id = updateAccountCommand.Id;
            account.Code = updateAccountCommand.Code;
            account.Label = updateAccountCommand.Label;
            account.Category = updateAccountCommand.Category;
            account.Domain = updateAccountCommand.Domain;
            account.Description = updateAccountCommand.Description;
            account.Version = updateAccountCommand.Version;
            account.CurrencyId = updateAccountCommand.CurrencyId;
            return account;
        }

        public static AccountAdded ToAccountAdded(this Account account)
        {
            return new AccountAdded
            {
                Code = account.Code,
                Label = account.Label,
                Category = account.Category,
                Domain = account.Domain,
                Description = account.Description,
                Version = account.Version,
                Id = account.Id,
                TenantId = account.TenantId,
                CurrencyId =account.CurrencyId
            };
        }

        public static AccountUpdated ToAccountUpdated(this Account account)
        {
            return new AccountUpdated
            {
                Code = account.Code,
                Label = account.Label,
                Category = account.Category,
                Domain = account.Domain,
                Description = account.Description,
                Version = account.Version,
                Id = account.Id,
                TenantId = account.TenantId,
                CurrencyId = account.CurrencyId
            };
        }
    }
}
