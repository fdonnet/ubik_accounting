using Ubik.Accounting.Contracts.AccountGroups.Events;
using Ubik.Accounting.Contracts.AccountGroups.Results;

namespace Ubik.Accounting.Contracts.Classifications.Events
{
    public record ClassificationDeleted
    {
        public Guid Id { get; init; }
        public IEnumerable<AccountGroupStandardResult> AccountGroupsDeleted { get; init; } = [];
    }
}
