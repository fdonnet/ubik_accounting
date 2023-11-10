using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Classifications.Results
{
    public record GetClassificationAccountsResults
    {
        public IEnumerable<GetClassificationAccountsResult> Accounts { get; init; } = default!;
    }
}
