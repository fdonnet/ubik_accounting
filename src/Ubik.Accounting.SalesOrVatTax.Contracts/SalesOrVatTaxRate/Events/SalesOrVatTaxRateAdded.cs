using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.SalesOrVatTax.Contracts.VatRate.Events
{
    public record SalesOrVatTaxRateAdded
    {
        public Guid Id { get; init; }
        public DateTime ValidFrom { get; init; }
        public DateTime? ValidTo { get; init; }
        public required string Code { get; init; }
        public string? Description { get; init; }
        public Decimal Rate { get; init; }
        public Guid Version { get; init; }
    }
}
