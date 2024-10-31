namespace Ubik.Accounting.Structure.Contracts.Accounts.Events
{
    public record AccountDeletedInAccountGroup
    {
        public Guid AccountId { get; init; }
        public Guid AccountGroupId { get; init; }
    }
}
