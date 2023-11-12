using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.AccountGroups.Events
{
    public record AccountGroupsDeleted
    {
        public IEnumerable<AccountGroupDeleted> AccountGroups { get; init; } = default!;
    }
}
