using MassTransit;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Contracts.AccountGroups.Events;
using Ubik.Accounting.Contracts.AccountGroups.Results;


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

        public static IEnumerable<DeleteAccountGroupResult> ToDeleteAccountGroupsResult(this IEnumerable<AccountGroup> accountGroups)
        {
            return accountGroups.Select(x => new DeleteAccountGroupResult()
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

        public static IEnumerable<AccountGroupDeleted> ToAccountGroupDeleted(this IEnumerable<AccountGroup> accountGroups)
        {
            return accountGroups.Select(x => new AccountGroupDeleted()
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

        public static AccountGroup ToAccountGroup(this UpdateAccountGroupCommand updAccountGroupCommand)
        {
            return new AccountGroup()
            {
                Id = updAccountGroupCommand.Id,
                Code = updAccountGroupCommand.Code,
                Label = updAccountGroupCommand.Label,
                Description = updAccountGroupCommand.Description,
                ParentAccountGroupId = updAccountGroupCommand.ParentAccountGroupId,
                AccountGroupClassificationId = updAccountGroupCommand.AccountGroupClassificationId,
                Version = updAccountGroupCommand.Version
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

        public static AccountGroup ToAccountGroup(this AccountGroup forUpd, AccountGroup accountGroup)
        {
            accountGroup.Id = forUpd.Id;
            accountGroup.Code = forUpd.Code;
            accountGroup.Label = forUpd.Label;
            accountGroup.Description = forUpd.Description;
            accountGroup.ParentAccountGroupId = forUpd.ParentAccountGroupId;
            accountGroup.AccountGroupClassificationId = forUpd.AccountGroupClassificationId;
            accountGroup.Version = forUpd.Version;
            return accountGroup;
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

        public static AccountGroupAdded ToAccountGroupAdded(this AccountGroup accountGroup)
        {
            return new AccountGroupAdded
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

        public static AccountGroupUpdated ToAccountGroupUpdated(this AccountGroup accountGroup)
        {
            return new AccountGroupUpdated
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

        public static AccountGroupDeleted ToAccountGroupDeleted(this AccountGroup accountGroup)
        {
            return new AccountGroupDeleted
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
