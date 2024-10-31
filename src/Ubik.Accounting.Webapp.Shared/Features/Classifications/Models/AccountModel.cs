using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Structure.Contracts.Accounts.Enums;
using Ubik.Accounting.Structure.Contracts.Accounts.Results;
using Ubik.Accounting.Structure.Contracts.Classifications.Results;

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

        public AccountModel Clone()
        {
            return new()
            {
                Id = Id,
                Code = Code,
                Label = Label,
                Description = Description,
                Category = Category,
                Domain = Domain,
                CurrencyId = CurrencyId,
                Version = Version
            };
        }
    }
    public static class AccountModelMappers
    {
        public static IEnumerable<AccountModel> ToAccountModels(this IEnumerable<AccountStandardResult> current)
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
