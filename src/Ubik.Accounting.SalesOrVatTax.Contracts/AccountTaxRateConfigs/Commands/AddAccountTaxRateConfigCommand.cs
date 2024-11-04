using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Commands
{
    public record AddAccountTaxRateConfigCommand
    {
        public Guid AccountId { get; init; }
        public Guid TaxRateId { get; init; }
        public Guid TaxAccountId { get; init; }
    }
}
