using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.SalesOrVatTax.Contracts.AccountLinkedTaxRates.Events
{
    public record AccountTaxRateConfigDeleted
    {
        public Guid Id { get; init; }
    }
}
