using MassTransit;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Accounts.Events;
using Ubik.Accounting.Contracts.Accounts.Results;

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
                Id = NewId.NextGuid(),
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

        public static Account ToAccount(this Account accountForUpd, Account account)
        {
            account.Id = accountForUpd.Id;
            account.Code = accountForUpd.Code;
            account.Label = accountForUpd.Label;
            account.Category = accountForUpd.Category;
            account.Domain = accountForUpd.Domain;
            account.Description = accountForUpd.Description;
            account.Version = accountForUpd.Version;
            account.CurrencyId = accountForUpd.CurrencyId;
            return account;
        }

        public static Account ToAccount(this UpdateAccountCommand updateAccountCommand)
        {
            return new Account()
            {
                Id = updateAccountCommand.Id,
                Code = updateAccountCommand.Code,
                Label = updateAccountCommand.Label,
                Category = updateAccountCommand.Category,
                Domain = updateAccountCommand.Domain,
                Description = updateAccountCommand.Description,
                Version = updateAccountCommand.Version,
                CurrencyId = updateAccountCommand.CurrencyId,
            };
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
                CurrencyId = account.CurrencyId
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
