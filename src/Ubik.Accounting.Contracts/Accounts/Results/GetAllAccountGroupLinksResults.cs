namespace Ubik.Accounting.Contracts.Accounts.Results
{
    public record GetAllAccountGroupLinksResults
    {
        public GetAllAccountGroupLinksResult[] AccountGroupLinks { get; init; } = default!;
    }
}
