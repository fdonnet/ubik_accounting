using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Classifications.Results
{
    public record GetClassificationAccountsMissingResults
    {
        public IEnumerable<GetClassificationAccountsMissingResult> Accounts { get; init; } = default!;
    }
}
