using MassTransit;
using Ubik.Accounting.Api.Models;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.AddAccountGroup;
using static Ubik.Accounting.Api.Features.AccountGroups.Commands.UpdateAccountGroup;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAccountGroup;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetAllAccountGroups;
using static Ubik.Accounting.Api.Features.AccountGroups.Queries.GetChildAccounts;


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
                AccountGroupClassificationId = x.AccountGroupClassificationId,
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
                AccountGroupClassificationId = accountGroup.AccountGroupClassificationId,
                Version = accountGroup.Version
            };
        }

        public static AddAccountGroupResult ToAddAccountGroupResult(this AccountGroup account)
        {
            return new AddAccountGroupResult()
            {
                Id = account.Id,
                Code = account.Code,
                Label = account.Label,
                Description = account.Description,
                ParentAccountGroupId = account.ParentAccountGroupId,
                AccountGroupClassificationId = account.AccountGroupClassificationId,
                Version = account.Version
            };
        }

        public static AccountGroup ToAccountGroup(this AddAccountGroupCommand addAccountGroupCommand)
        {
            return new AccountGroup()
            {
                Id = NewId.NextGuid(),
                Code = addAccountGroupCommand.Code,
                Label = addAccountGroupCommand.Label,
                Description = addAccountGroupCommand.Description,
                ParentAccountGroupId = addAccountGroupCommand.ParentAccountGroupId,
                AccountGroupClassificationId = addAccountGroupCommand.AccountGroupClassificationId,
            };
        }

        public static UpdateAccountGroupResult ToUpdateAccountGroupResult(this AccountGroup accountGroup)
        {
            return new UpdateAccountGroupResult()
            {
                Id = accountGroup.Id,
                Code = accountGroup.Code,
                Label = accountGroup.Label,
                Description = accountGroup.Description,
                ParentAccountGroupId = accountGroup.ParentAccountGroupId,
                AccountGroupClassificationId = accountGroup.AccountGroupClassificationId,
                Version = accountGroup.Version
            };
        }

        public static AccountGroup ToAccountGroup(this UpdateAccountGroupCommand updateAccountGroupCommand, AccountGroup accountGroup)
        {
            accountGroup.Id = updateAccountGroupCommand.Id;
            accountGroup.Code = updateAccountGroupCommand.Code;
            accountGroup.Label = updateAccountGroupCommand.Label;
            accountGroup.Description = updateAccountGroupCommand.Description;
            accountGroup.ParentAccountGroupId = updateAccountGroupCommand.ParentAccountGroupId;
            accountGroup.AccountGroupClassificationId = updateAccountGroupCommand.AccountGroupClassificationId;
            accountGroup.Version = updateAccountGroupCommand.Version;
            return accountGroup;
        }

        public static IEnumerable<GetChildAccountsResult> ToGetChildAccountsResult(this IEnumerable<Account> accounts)
        {
            return accounts.Select(x => new GetChildAccountsResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                Version = x.Version
            });
        }
    }
}
