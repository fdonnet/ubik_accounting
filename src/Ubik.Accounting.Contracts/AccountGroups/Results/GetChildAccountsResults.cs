namespace Ubik.Accounting.Contracts.AccountGroups.Results
{
    public record GetChildAccountsResults
    {
        public GetChildAccountsResult[] ChildAccounts { get; init; } = default!;
    }
}
