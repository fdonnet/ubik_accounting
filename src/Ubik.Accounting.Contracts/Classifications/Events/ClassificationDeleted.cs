using Ubik.Accounting.Contracts.AccountGroups.Events;

namespace Ubik.Accounting.Contracts.Classifications.Events
{
    public record ClassificationDeleted
    {
        public Guid Id { get; init; }
        public IEnumerable<AccountGroupDeleted> AccountGroupsDeleted { get; init; } = [];
    }
}
