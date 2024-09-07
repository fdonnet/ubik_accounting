﻿using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Contracts.Accounts.Enums;
using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.WebApp.Client.Components.Classifications
{
    public class AccountModel
    {
        public Guid Id { get; init; }
        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = default!;
        [Required]
        [MaxLength(100)]
        public string Label { get; set; } = default!;
        [MaxLength(700)]
        public string? Description { get; set; }
        [EnumDataType(typeof(AccountCategory))]
        [Required]
        public AccountCategory? Category { get; set; } = null;
        [EnumDataType(typeof(AccountDomain))]
        [Required]
        public AccountDomain? Domain { get; set; } = null;
        [Required]
        public Guid? CurrencyId { get; set; }
        public Guid Version { get; init; }
    }
    public static class AccountModelMappers
    {
        public static IEnumerable<AccountModel> ToAccountModel(this IEnumerable<GetAllAccountsResult> current)
        {
            return current.Select(x => new AccountModel()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                Category = x.Category,
                Domain = x.Domain,
                CurrencyId = x.CurrencyId,
                Version = x.Version
            });
        }
    }
}