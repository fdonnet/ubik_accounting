using MassTransit;
using Ubik.Accounting.Api.Features.Accounts.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Accounts.Events;
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

        public static IEnumerable<AccountGroupWithClassificationResult> ToAccountGroupWithClassificationResult(this IEnumerable<AccountGroupClassification> accountGroupClassification)
        {
            return accountGroupClassification.Select(x => new AccountGroupWithClassificationResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                ClassificationId = x.ClassificationId,
                ClassificationCode = x.ClassificationCode,
                CLassificationLabel = x.ClassificationLabel
            });
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

        //public static Account ToAccount(this UpdateAccountCommand updateAccountCommand, Account account)
        //{
        //    account.Id = updateAccountCommand.Id;
        //    account.Code = updateAccountCommand.Code;
        //    account.Label = updateAccountCommand.Label;
        //    account.Category = updateAccountCommand.Category;
        //    account.Domain = updateAccountCommand.Domain;
        //    account.Description = updateAccountCommand.Description;
        //    account.Version = updateAccountCommand.Version;
        //    account.CurrencyId = updateAccountCommand.CurrencyId;
        //    return account;
        //}

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

        public static AccountAccountGroup ToAccountAccountGroup(this AddAccountInAccountGroupCommand addAccountInAccountGroupCommand)
        {
            return new AccountAccountGroup()
            {
                Id = NewId.NextGuid(),
                AccountId = addAccountInAccountGroupCommand.AccountId,
                AccountGroupId = addAccountInAccountGroupCommand.AccountGroupId
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

        public static AccountInAccountGroupResult ToAccountInAccountGroupStandardResult(this AccountAccountGroup accountAccountGroup)
        {
            return new AccountInAccountGroupResult
            {
                Id = accountAccountGroup.AccountId,
                AccountId = accountAccountGroup.AccountId,
                AccountGroupId = accountAccountGroup.AccountGroupId,
                Version = accountAccountGroup.Version,
            };
        }
    }
}
