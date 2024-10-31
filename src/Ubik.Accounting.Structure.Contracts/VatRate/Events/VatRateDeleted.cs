using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Structure.Contracts.VatRate.Events
{
    public record VatRateDeleted
    {
        public Guid Id { get; init; }
    }
}
