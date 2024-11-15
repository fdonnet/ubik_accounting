namespace Ubik.Accounting.Structure.Contracts.AccountGroups.Results
{
    public record AccountGroupStandardResult
    {
        public Guid Id { get; init; }
        public required string Code { get; init; }
        public required string Label { get; init; }
        public string? Description { get; init; }
        public Guid? ParentAccountGroupId { get; init; }
        public Guid AccountGroupClassificationId { get; init; }
        public Guid Version { get; init; }
    }
}
