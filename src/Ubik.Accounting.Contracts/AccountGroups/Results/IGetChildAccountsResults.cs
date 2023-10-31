using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.AccountGroups.Results
{
    public interface IGetChildAccountsResults
    {
        GetChildAccountsResult[] ChildAccounts { get; }
    }
}
