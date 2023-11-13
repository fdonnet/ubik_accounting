using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Accounts.Events
{
    public record AccountDeletedInAccountGroup
    {
        public Guid AccountId { get; init; }
        public Guid AccountGroupId { get; init; }
    }
}
