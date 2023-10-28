using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.ApiService.DB.Enums;

namespace Ubik.Accounting.Contracts.Accounts.Commands
{
    public record AddAccountCommand
    {
        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = default!;
        [Required]
        [MaxLength(100)]
        public string Label { get; set; } = default!;
        [MaxLength(700)]
        public string? Description { get; set; }
        [JsonRequired]
        [EnumDataType(typeof(AccountCategory))]
        public AccountCategory Category { get; set; }
        [JsonRequired]
        [EnumDataType(typeof(AccountDomain))]
        public AccountDomain Domain { get; set; }
        [Required]
        public Guid CurrencyId { get; set; }
    }
}
