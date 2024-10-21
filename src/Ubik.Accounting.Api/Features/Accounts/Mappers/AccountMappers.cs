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

        public static AccountDeletedInAccountGroup ToAccountDeletedInAccountGroup(this AccountAccountGroup accountAccountGroup)
        {
            return new AccountDeletedInAccountGroup
            {
                AccountId = accountAccountGroup.AccountId,
                AccountGroupId = accountAccountGroup.AccountGroupId
            };
        }

        public static AccountInAccountGroupStandardResult ToAddAccountInAccountGroupResult(this AccountAccountGroup accountAccountGroup)
        {
            return new AccountInAccountGroupStandardResult
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
