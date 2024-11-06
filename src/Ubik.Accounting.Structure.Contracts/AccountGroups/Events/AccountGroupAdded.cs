namespace Ubik.Accounting.Structure.Contracts.AccountGroups.Events
{
    public record AccountGroupAdded
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = default!;
        public string Label { get; init; } = default!;
        public string? Description { get; init; }
        public Guid? ParentAccountGroupId { get; init; }
        public Guid AccountGroupClassificationId { get; init; }
        public Guid Version { get; init; }
        public Guid TenantId { get; init; }
    }
}
