namespace Ubik.Accounting.Contracts.Accounts.Results
{
    public record GetAllAccountGroupLinksResult
    {
        public Guid Id { get; init; }
        public Guid AccountId { get; init; }
        public Guid AccountGroupId { get; init; }
        public Guid Version { get; init; }
    }
}
