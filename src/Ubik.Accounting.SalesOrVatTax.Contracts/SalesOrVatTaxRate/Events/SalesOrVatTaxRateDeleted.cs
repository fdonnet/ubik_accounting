using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.SalesOrVatTax.Contracts.SalesOrVatTaxRate.Events
{
    public record SalesOrVatTaxRateDeleted
    {
        public Guid Id { get; init; }
    }
}
