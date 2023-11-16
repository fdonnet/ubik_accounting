namespace Ubik.Accounting.Contracts.Accounts.Results
{
    public record GetAccountGroupClassificationResults
    {
        public IEnumerable<GetAccountGroupClassificationResult> AccountGroups { get; init; } = default!;
    }
}
