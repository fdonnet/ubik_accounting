using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Contracts.Accounts.Enums;

namespace Ubik.Accounting.WebApp.Client.Components.Accounts
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
        public Guid CurrencyId { get; set; }
        public Guid Version { get; init; }
    }
}
