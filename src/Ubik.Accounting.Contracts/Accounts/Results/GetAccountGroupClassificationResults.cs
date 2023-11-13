using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Accounts.Results
{
    public record GetAccountGroupClassificationResults
    {
        public IEnumerable<GetAccountGroupClassificationResult> AccountGroups { get; init; } = default!;
    }
}
