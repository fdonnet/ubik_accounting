namespace Ubik.Accounting.Contracts.Accounts.Results
{
    public record GetAllAccountsResults
    {
        public GetAllAccountsResult[] Accounts { get; init; } = default!;
    }
}
