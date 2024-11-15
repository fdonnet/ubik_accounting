using Ubik.Accounting.Structure.Contracts.AccountGroups.Results;

namespace Ubik.Accounting.Structure.Contracts.Classifications.Events
{
    public record ClassificationDeleted
    {
        public Guid Id { get; init; }
        public IEnumerable<AccountGroupStandardResult> AccountGroupsDeleted { get; init; } = [];
    }
}
