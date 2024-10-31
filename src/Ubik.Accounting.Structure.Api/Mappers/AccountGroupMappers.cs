using MassTransit;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Contracts.AccountGroups.Events;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.Accounting.Contracts.Accounts.Events;

namespace Ubik.Accounting.Structure.Api.Mappers
{
    public static class AccountGroupMappers
    {
        public static IEnumerable<AccountGroupStandardResult> ToAccountGroupStandardResults(this IEnumerable<AccountGroup> accountGroups)
        {
            return accountGroups.Select(x => new AccountGroupStandardResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                ParentAccountGroupId = x.ParentAccountGroupId,
                AccountGroupClassificationId = x.ClassificationId,
                Version = x.Version
            });
        }

        public static AccountGroupStandardResult ToAccountGroupStandardResult(this AccountGroup accountGroup)
        {
            return new AccountGroupStandardResult()
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

        public static AccountGroup ToAccountGroup(this AddAccountGroupCommand addAccountGroupCommand)
        {
            return new AccountGroup()
            {
                Id = NewId.NextGuid(),
                Code = addAccountGroupCommand.Code,
                Label = addAccountGroupCommand.Label,
                Description = addAccountGroupCommand.Description,
                ParentAccountGroupId = addAccountGroupCommand.ParentAccountGroupId,
                ClassificationId = addAccountGroupCommand.AccountGroupClassificationId,
            };
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

        public static AccountDeletedInAccountGroup ToAccountDeletedInAccountGroup(this AccountAccountGroup accountAccountGroup)
        {
            return new AccountDeletedInAccountGroup
            {
                AccountId = accountAccountGroup.AccountId,
                AccountGroupId = accountAccountGroup.AccountGroupId
            };
        }
    }
}
