namespace Ubik.Accounting.Contracts.AccountGroups.Events
{
    public record AccountGroupsDeleted
    {
        public IEnumerable<AccountGroupDeleted> AccountGroups { get; init; } = default!;
    }
}
