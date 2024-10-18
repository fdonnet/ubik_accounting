namespace Ubik.Accounting.Contracts.AccountGroups.Results
{
    public record GetAllAccountGroupsResults
    {
        public AccountGroupStandardResult[] AccountGroups { get; init; } = default!;
    }
}

