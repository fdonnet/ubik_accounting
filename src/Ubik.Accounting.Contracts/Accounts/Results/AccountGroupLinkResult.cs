using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Accounting.Contracts.Accounts.Results
{
    public record AccountGroupLinkResult
    {
        public Guid Id { get; init; }
        public Guid AccountId { get; init; }
        public Guid AccountGroupId { get; init; }
        public Guid Version { get; init; }
    }
}
