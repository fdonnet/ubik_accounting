using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.Accounting.Contracts.Classifications.Results;
using Ubik.Accounting.Webapp.Shared.Facades;
using Ubik.Accounting.Webapp.Shared.Features.Classifications.Models;


namespace Ubik.Accounting.Webapp.Shared.Features.Classifications.Services
{
    public class ClassificationStateService
    {
        public event EventHandler<AccountGrpArgs>? OnChangeData;
        public event EventHandler? OnChangeClassification;


        public List<AccountGroupModel> AccountGroups { get; private set; } = default!;
        public List<AccountGroupModel> AccountGroupsRoot { get; private set; } = [];
        public Dictionary<Guid, List<AccountGroupModel>> AccountGroupsDicByParent { get; private set; } = default!;
        public Dictionary<Guid, AccountModel> Accounts { get; private set; } = default!;
        public Dictionary<Guid, List<AccountGroupLinkModel>> AccountsLinksByParent { get; private set; } = default!;
        public Dictionary<Guid, AccountModel> CurrentClassificationMissingAccounts { get; private set; } = default!;

        public void RefreshAccountGroupsRoot(Guid? classificationId)
        {
            AccountGroupsRoot = [.. AccountGroups.Where(ag => ag.AccountGroupClassificationId == classificationId
                                                           && ag.ParentAccountGroupId == null).OrderBy(root => root.Code)];
        }

        public void SetClassificationMissingAccounts(List<AccountModel> accounts)
        {
            CurrentClassificationMissingAccounts = accounts.ToDictionary(x => x.Id, x => x);
            NotifyClassificationChanged();
        }

        public void SetAccountGroups(IEnumerable<AccountGroupModel> accountGroups)
        {
            AccountGroups = accountGroups.ToList();
            BuildAccountGrpDicByParent();
        }

        public void SetAccounts(Dictionary<Guid, AccountModel> accounts)
        {
            Accounts = accounts;
        }

        public void ToggleExpandHideAllAccountGroups(bool expand)
        {
            AccountGroups.ForEach(ag => ag.IsExpand = expand);
            BuildAccountGrpDicByParent();
        }

        public void BuildAccountLinksByParentDic(IEnumerable<AccountGroupLinkModel> links)
        {
            AccountsLinksByParent = [];

            foreach (var childAccount in links)
            {
                if (AccountsLinksByParent.TryGetValue((Guid)childAccount.AccountGroupId!, out var value))
                    value.Add(childAccount);
                else
                    AccountsLinksByParent.Add((Guid)childAccount.AccountGroupId!, [childAccount]);
            }
        }

        public void AddAccountGroup(AccountGroupModel accountGroup)
        {
            if (accountGroup.ParentAccountGroupId != null)
            {
                if (AccountGroupsDicByParent.TryGetValue((Guid)accountGroup.ParentAccountGroupId, out var current))
                {
                    current.Add(accountGroup);
                }
                else
                    AccountGroupsDicByParent.Add((Guid)accountGroup.ParentAccountGroupId, [accountGroup]);
            }

            AccountGroups.Add(accountGroup);

            NotifyStateChanged(accountGroup.Id, AccountGrpArgsType.Added);
        }

        public void EditAccountGroup(AccountGroupModel accountGroup)
        {
            if (accountGroup.ParentAccountGroupId != null)
            {
                if (AccountGroupsDicByParent.TryGetValue((Guid)accountGroup.ParentAccountGroupId, out var current))
                {
                    current[current.FindIndex(a => a.Id == accountGroup.Id)] = accountGroup;
                }
            }

            AccountGroups[AccountGroups.FindIndex(a => a.Id == accountGroup.Id)] = accountGroup;

            NotifyStateChanged(accountGroup.Id, AccountGrpArgsType.Edited);
        }

        public void AttachAccountInAccountGroup(AccountGroupLinkModel accountGroupLink)
        {
            CurrentClassificationMissingAccounts.Remove(accountGroupLink.AccountId);

            if (AccountsLinksByParent.TryGetValue(accountGroupLink.AccountGroupId, out var current))
                current.Add(accountGroupLink);
            else
            {
                AccountsLinksByParent.Add(accountGroupLink.AccountGroupId, [accountGroupLink]);
            }
        }

        public void DetachAccountInAccountGroup(AccountGroupLinkModel accountGroupLink)
        {
            if (Accounts.TryGetValue(accountGroupLink.AccountId, out var currentAccount)
                && CurrentClassificationMissingAccounts.TryGetValue(accountGroupLink.AccountId, out var badAccount) == false)
            {
                CurrentClassificationMissingAccounts.Add(accountGroupLink.AccountId, currentAccount.Clone());

                if (AccountsLinksByParent.TryGetValue(accountGroupLink.AccountGroupId, out var parentLink))
                {
                    parentLink.RemoveAt(parentLink.FindIndex(a => a.AccountId == accountGroupLink.AccountId));

                    if (parentLink.Count == 0)
                        AccountsLinksByParent.Remove(accountGroupLink.AccountGroupId);

                    NotifyStateChanged(accountGroupLink.AccountGroupId, AccountGrpArgsType.AccountRemoved);
                }
                else
                {
                    throw new InvalidOperationException($"The account {accountGroupLink.AccountId} " +
                    $"is not attached anywhere. Pls refresh.");
                }
            }
            else
            {
                throw new InvalidOperationException($"The account {accountGroupLink.AccountId} " +
                    $"you are trying to attach doesn't exist locally or error in data. Pls refresh.");
            }
        }

        public void RemoveAccountGroup(Guid accountGroupId)
        {
            AccountGroupsDicByParent.Remove(accountGroupId);
            AccountGroups.RemoveAt(AccountGroups.FindIndex(a => a.Id == accountGroupId));

            if (AccountsLinksByParent.TryGetValue(accountGroupId, out var accountLinks))
            {
                foreach (var accountLink in accountLinks)
                {
                    if (Accounts.TryGetValue(accountLink.AccountId, out var account))
                    {
                        CurrentClassificationMissingAccounts.Add(account.Id, account.Clone());
                    }
                }
            }
            AccountsLinksByParent.Remove(accountGroupId);
            NotifyStateChanged(accountGroupId, AccountGrpArgsType.Deleted);
        }

        private void NotifyStateChanged(Guid accountGroupId, AccountGrpArgsType type) => OnChangeData?.Invoke(this, new AccountGrpArgs(accountGroupId, type));

        private void NotifyClassificationChanged() => OnChangeClassification?.Invoke(this, EventArgs.Empty);

        private void BuildAccountGrpDicByParent()
        {
            var withParent = AccountGroups.Where(ag => ag.ParentAccountGroupId != null);
            AccountGroupsDicByParent = [];

            foreach (var childAccountGrp in withParent)
            {
                if (AccountGroupsDicByParent.TryGetValue((Guid)childAccountGrp.ParentAccountGroupId!, out var value))
                    value.Add(childAccountGrp);
                else
                    AccountGroupsDicByParent.Add((Guid)childAccountGrp.ParentAccountGroupId!, [childAccountGrp]);
            }
        }

    }

    public class AccountGrpArgs(Guid accountGroupId, AccountGrpArgsType type) : EventArgs
    {
        public Guid AccountGroupId { get; } = accountGroupId;
        public AccountGrpArgsType Type { get; } = type;
    }
    public enum AccountGrpArgsType
    {
        Added,
        Edited,
        Deleted,
        AccountRemoved
    }


}
