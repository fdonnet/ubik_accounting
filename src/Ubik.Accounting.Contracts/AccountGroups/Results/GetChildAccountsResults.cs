using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.AccountGroups.Results
{
    public record GetChildAccountsResults
    {
        public GetChildAccountsResult[] ChildAccounts { get; init; } = default!;
    }
}
