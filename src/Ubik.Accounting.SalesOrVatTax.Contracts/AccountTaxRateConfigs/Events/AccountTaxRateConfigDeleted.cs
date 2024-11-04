using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Events
{
    public record AccountTaxRateConfigDeleted
    {
        public Guid Id { get; init; }
    }
}
