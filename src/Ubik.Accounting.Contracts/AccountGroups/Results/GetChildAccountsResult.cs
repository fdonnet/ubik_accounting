namespace Ubik.Accounting.Contracts.AccountGroups.Results
{
    public record GetChildAccountsResult
    {
        public Guid Id { get; init; }
        public required string Code { get; init; }
        public required string Label { get; init; }
        public string? Description { get; init; }
        public Guid Version { get; init; }
    }
}
