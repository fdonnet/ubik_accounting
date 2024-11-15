namespace Ubik.Accounting.Structure.Contracts.Accounts.Results
{
    public record AccountGroupLinkResult
    {
        public Guid Id { get; init; }
        public Guid AccountId { get; init; }
        public Guid AccountGroupId { get; init; }
        public Guid Version { get; init; }
    }
}
