using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Classifications.Results
{
    public class GetClassificationStatusResult
    {
        public Guid Id { get; init; }
        public bool IsReady { get; init; }
        public IEnumerable<GetClassificationAccountsMissingResult> MissingAccounts { get; init; } = default!;
    }
}
