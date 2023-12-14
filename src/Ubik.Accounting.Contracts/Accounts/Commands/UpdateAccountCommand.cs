﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Ubik.Accounting.Contracts.Accounts.Enums;

namespace Ubik.Accounting.Contracts.Accounts.Commands
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
        [Required]
        public Guid Version { get; init; }
        [Required]
        public Guid CurrencyId { get; init; }
    }
}
