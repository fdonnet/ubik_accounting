using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Ubik.Accounting.Structure.Contracts.Accounts.Enums;

namespace Ubik.Accounting.Structure.Contracts.Accounts.Commands
{
    public record UpdateAccountCommand
    {
        [Required]
        public Guid Id { get; init; }
        [Required]
        [MaxLength(20)]
        public string Code { get; init; } = default!;
        [Required]
        [MaxLength(100)]
        public string Label { get; init; } = default!;
        [JsonRequired]
        [EnumDataType(typeof(AccountCategory))]
        public AccountCategory Category { get; init; }
        [JsonRequired]
        [EnumDataType(typeof(AccountDomain))]
        public AccountDomain Domain { get; init; }
        [MaxLength(700)]
        public string? Description { get; init; }
        public bool Active { get; init; } = true;
        [Required]
        public Guid Version { get; init; }
        [Required]
        public Guid CurrencyId { get; init; }
    }
}
