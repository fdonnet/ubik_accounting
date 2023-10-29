using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Ubik.ApiService.DB.Enums;

namespace Ubik.Accounting.Contracts.Accounts.Commands
{
    public record UpdateAccountCommand
    {
        [Required]
        [JsonIgnore]
        public Guid Id { get; set; }
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
        [Required]
        public Guid Version { get; init; }
        [Required]
        public Guid CurrencyId { get; init; }
    }
}
