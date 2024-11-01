using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.SalesOrVatTax.Contracts.VatRate.Events
{
    public record SalesOrVatTaxRateDeleted
    {
        public Guid Id { get; init; }
    }
}
