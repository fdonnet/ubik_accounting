using MassTransit;
using Ubik.Accounting.Api.Features.Accounts.CustomPoco;
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

        public static AccountAccountGroup ToAccountAccountGroup(this AddAccountInAccountGroupCommand addAccountInAccountGroupCommand)
        {
            return new AccountAccountGroup()
            {
                Id = NewId.NextGuid(),
                AccountId = addAccountInAccountGroupCommand.AccountId,
                AccountGroupId = addAccountInAccountGroupCommand.AccountGroupId
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

        public static AccountAddedInAccountGroup ToAccountAddedInAccountGroup(this AccountAccountGroup accountAccountGroup)
        {
            return new AccountAddedInAccountGroup
            {
               AccountId = accountAccountGroup.AccountId,
               AccountGroupId = accountAccountGroup.AccountGroupId
            };
        }

        public static AccountDeletedInAccountGroup ToAccountDeletedInAccountGroup(this AccountAccountGroup accountAccountGroup)
        {
            return new AccountDeletedInAccountGroup
            {
                AccountId = accountAccountGroup.AccountId,
                AccountGroupId = accountAccountGroup.AccountGroupId
            };
        }

        public static AddAccountInAccountGroupResult ToAddAccountInAccountGroupResult(this AccountAccountGroup accountAccountGroup)
        {
            return new AddAccountInAccountGroupResult
            {
                Id = accountAccountGroup.AccountId,
                AccountId = accountAccountGroup.AccountId,
                AccountGroupId = accountAccountGroup.AccountGroupId,
                Version = accountAccountGroup.Version,
            };
        }

        public static DeleteAccountInAccountGroupResult ToDeleteAccountInAccountGroupResult(this AccountAccountGroup accountAccountGroup)
        {
            return new DeleteAccountInAccountGroupResult
            {
                AccountId = accountAccountGroup.AccountId,
                AccountGroupId = accountAccountGroup.AccountGroupId
            };
        }


    }
}
