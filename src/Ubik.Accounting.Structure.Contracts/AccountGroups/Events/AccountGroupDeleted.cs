namespace Ubik.Accounting.Structure.Contracts.AccountGroups.Events
{
    public record AccountGroupDeleted
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = default!;
        public string Label { get; init; } = default!;
        public Guid? ParentAccountGroupId { get; init; }
        public Guid TenantId { get; init; }
    }
}
