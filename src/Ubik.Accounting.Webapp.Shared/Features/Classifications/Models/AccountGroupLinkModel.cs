﻿using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.Webapp.Shared.Features.Classifications.Models
{
    public class AccountGroupLinkModel
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid AccountGroupId { get; set; }
        public Guid Version { get; set; }
    }

    public static class AccountGroupLinkModelMappers
    {
        public static IEnumerable<AccountGroupLinkModel> ToAccountGroupLinkModels(this IEnumerable<GetAllAccountGroupLinksResult> current)
        {
            return current.Select(x => new AccountGroupLinkModel()
            {
                Id = x.Id,
                AccountGroupId = x.AccountGroupId,
                AccountId = x.AccountId,    
                Version = x.Version
            });
        }

        public static AccountGroupLinkModel ToAccountGroupLinkModel(this AddAccountInAccountGroupResult current)
        {
            return new()
            {
                Id=current.Id,
                AccountGroupId = current.AccountGroupId,
                AccountId = current.AccountId,
                Version = current.Version
            };
        }
    }
}
