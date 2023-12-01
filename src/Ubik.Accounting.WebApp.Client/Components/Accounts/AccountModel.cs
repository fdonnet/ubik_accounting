using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Ubik.Accounting.Contracts.Accounts.Enums;

namespace Ubik.Accounting.WebApp.Client.Components.Accounts
{
    public class AccountModel
    {
        public Guid Id { get; init; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(20, MinimumLength =1)]
        [MaxLength(20)]
        public string Code { get; set; } = default!;
        [Required]
        [MaxLength(100)]
        public string Label { get; set; } = default!;
        [MaxLength(700)]
        public string? Description { get; set; }
        [EnumDataType(typeof(AccountCategory))]
        public AccountCategory Category { get; set; }
        [EnumDataType(typeof(AccountDomain))]
        public AccountDomain Domain { get; set; }
        [Required]
        public Guid CurrencyId { get; set; }
        public Guid Version { get; init; }
    }
}
