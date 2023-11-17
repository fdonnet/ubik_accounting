using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.Contracts.AccountGroups.Events;
using Ubik.Accounting.Contracts.AccountGroups.Results;

namespace Ubik.Accounting.Contracts.Classifications.Events
{
    public record ClassificationDeleted
    {
        public Guid Id { get; init; }
        public IEnumerable<AccountGroupDeleted> AccountGroupsDeleted { get; init; } = [];
    }
}
