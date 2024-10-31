using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Structure.Contracts.Accounts.Enums;

namespace Ubik.Accounting.Structure.Contracts.Accounts.Results
{
    public record AccountStandardResult
    {
        public Guid Id { get; init; }
        public required string Code { get; init; }
        public required string Label { get; init; }
        public AccountCategory Category { get; init; }
        public AccountDomain Domain { get; init; }
        public string? Description { get; init; }
        public bool Active { get; init; } = true;
        public Guid CurrencyId { get; init; }
        public Guid Version { get; init; }
    }
}
