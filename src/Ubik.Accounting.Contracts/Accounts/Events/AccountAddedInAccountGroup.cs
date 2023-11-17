namespace Ubik.Accounting.Contracts.Accounts.Events
{
    public record AccountAddedInAccountGroup
    {
        public Guid AccountId { get; init; }
        public Guid AccountGroupId { get; init; }
    }
}
