using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.Accounting.Contracts.Accounts.Enums;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.Accounting.Contracts.Classifications.Results;

namespace Ubik.Accounting.Webapp.Shared.Features.Classifications.Models
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
        public static IEnumerable<AccountModel> ToAccountModels(this IEnumerable<GetAllAccountsResult> current)
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

        public static IEnumerable<AccountModel> ToAccountModels(this IEnumerable<GetClassificationAccountsResult> current)
        {
            return current.Select(x => new AccountModel()
            {
                Id = x.Id,
                Label = x.Label,
                Description = x.Description,
                Category = x.Category,
                Domain = x.Domain,
                CurrencyId = x.CurrencyId,
                Code = x.Code,
                Version = x.Version
            });
        }
    }
}
