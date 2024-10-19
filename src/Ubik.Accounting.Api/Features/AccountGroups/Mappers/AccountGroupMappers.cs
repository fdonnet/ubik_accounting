using MassTransit;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Contracts.AccountGroups.Events;
using Ubik.Accounting.Contracts.AccountGroups.Results;


namespace Ubik.Accounting.Api.Features.AccountGroups.Mappers
{
    public static class AccountGroupMappers
    {
        public static IEnumerable<DeleteAccountGroupResult> ToDeleteAccountGroupsResult(this IEnumerable<AccountGroup> accountGroups)
        {
            return accountGroups.Select(x => new DeleteAccountGroupResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                ParentAccountGroupId = x.ParentAccountGroupId,
            });
        }

        public static IEnumerable<AccountGroupDeleted> ToAccountGroupDeleted(this IEnumerable<AccountGroup> accountGroups)
        {
            return accountGroups.Select(x => new AccountGroupDeleted()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                ParentAccountGroupId = x.ParentAccountGroupId,
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
                AccountGroupClassificationId = accountGroup.ClassificationId,
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
                AccountGroupClassificationId = account.ClassificationId,
                Version = account.Version
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
                ClassificationId = updAccountGroupCommand.AccountGroupClassificationId,
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
                AccountGroupClassificationId = accountGroup.ClassificationId,
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
            accountGroup.ClassificationId = forUpd.ClassificationId;
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
            accountGroup.ClassificationId = updateAccountGroupCommand.AccountGroupClassificationId;
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
                AccountGroupClassificationId = accountGroup.ClassificationId,
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
                AccountGroupClassificationId = accountGroup.ClassificationId,
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
                ParentAccountGroupId = accountGroup.ParentAccountGroupId,
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
