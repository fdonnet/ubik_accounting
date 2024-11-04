using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.SalesOrVatTax.Contracts.AccountLinkedTaxRates.Events
{
    public record AccountTaxRateConfigAdded
    {
        public Guid Id { get; init; }
        public Guid AccountId { get; init; }
        public Guid TaxRateId { get; init; }
        public Guid TaxAccountId { get; init; }
        public Guid Version { get; init; }
    }
}
