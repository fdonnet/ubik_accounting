using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Commands
{
    public record DeleteAccountTaxRateConfigCommand
    {
        public Guid AccountId { get; init; }
        public Guid TaxRateId { get; init; }
    }
}
