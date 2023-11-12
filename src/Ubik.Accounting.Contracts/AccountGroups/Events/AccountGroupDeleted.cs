using Ubik.Accounting.Contracts.AccountGroups.Results;

namespace Ubik.Accounting.Contracts.AccountGroups.Events
{
    public record AccountGroupDeleted
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = default!;
        public string Label { get; init; } = default!;
        public string? Description { get; init; }
        public Guid? ParentAccountGroupId { get; init; }
        public Guid AccountGroupClassificationId { get; init; }
        public Guid Version { get; init; }
    }

    public record AccountGroupsDeleted
    {
        public IEnumerable<AccountGroupDeleted> AccountGroups { get; init; } = default!;
    }
}
