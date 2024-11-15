namespace Ubik.Accounting.Structure.Contracts.Accounts.Events
{
    public record AccountAddedInAccountGroup
    {
        public Guid AccountId { get; init; }
        public Guid AccountGroupId { get; init; }
    }
}
