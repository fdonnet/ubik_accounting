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
        public event EventHandler<AccountGrpArgs>? OnChange;
        public List<AccountGroupModel> AccountGroups { get; private set; } = default!;
        public List<AccountGroupModel> AccountGroupsRoot { get; private set; } = [];
        public Dictionary<Guid, List<AccountGroupModel>> AccountGroupsDicByParent { get; private set; } = default!;
        public Dictionary<Guid, AccountModel> Accounts { get; private set; } = default!;
        public Dictionary<Guid, List<AccountGroupLinkModel>> AccountsLinksByParent { get; private set; } = default!;

        public void RefreshAccountGroupsRoot(Guid? classificationId)
        {
            AccountGroupsRoot = AccountGroups.Where(ag => ag.AccountGroupClassificationId == classificationId
                                                           && ag.ParentAccountGroupId == null).OrderBy(root => root.Code).ToList();
        }

        public void SetAccountGroups(List<AccountGroupModel> accountGroups)
        {
            AccountGroups = accountGroups;
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
            AccountsLinksByParent = new();

            foreach (var childAccount in links)
            {
                if (AccountsLinksByParent.TryGetValue((Guid)childAccount.AccountGroupId!, out var value))
                    value.Add(childAccount);
                else
                    AccountsLinksByParent.Add((Guid)childAccount.AccountGroupId!, new List<AccountGroupLinkModel> { childAccount });
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
                    AccountGroupsDicByParent.Add((Guid)accountGroup.ParentAccountGroupId, new() { accountGroup });
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

        public void RemoveAccountGroup(Guid accountGroupId)
        {
            AccountGroupsDicByParent.Remove(accountGroupId);
            AccountGroups.RemoveAt(AccountGroups.FindIndex(a => a.Id == accountGroupId));
            AccountsLinksByParent.Remove(accountGroupId);
            NotifyStateChanged(accountGroupId, AccountGrpArgsType.Deleted);
        }

        private void NotifyStateChanged(Guid accountGroupId, AccountGrpArgsType type) => OnChange?.Invoke(this, new AccountGrpArgs(accountGroupId, type));

        private void BuildAccountGrpDicByParent()
        {
            var withParent = AccountGroups.Where(ag => ag.ParentAccountGroupId != null);
            AccountGroupsDicByParent = new();

            foreach (var childAccountGrp in withParent)
            {
                if (AccountGroupsDicByParent.TryGetValue((Guid)childAccountGrp.ParentAccountGroupId!, out var value))
                    value.Add(childAccountGrp);
                else
                    AccountGroupsDicByParent.Add((Guid)childAccountGrp.ParentAccountGroupId!, new List<AccountGroupModel> { childAccountGrp });
            }
        }

    }

    public class AccountGrpArgs : EventArgs
    {
        public Guid AccountGroupId { get; }
        public AccountGrpArgsType Type { get; }

        public AccountGrpArgs(Guid accountGroupId, AccountGrpArgsType type)
        {
            AccountGroupId = accountGroupId;
            Type = type;
        }
    }
    public enum AccountGrpArgsType
    {
        Added,
        Edited,
        Deleted
    }


}
