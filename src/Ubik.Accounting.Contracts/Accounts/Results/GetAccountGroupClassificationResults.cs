namespace Ubik.Accounting.Contracts.Accounts.Results
{
    public record GetAccountGroupClassificationResults
    {
        public IEnumerable<AccountGroupWithClassificationResult> AccountGroups { get; init; } = default!;
    }
}
