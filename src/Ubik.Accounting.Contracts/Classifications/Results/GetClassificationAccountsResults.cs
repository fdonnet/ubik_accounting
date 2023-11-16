namespace Ubik.Accounting.Contracts.Classifications.Results
{
    public record GetClassificationAccountsResults
    {
        public IEnumerable<GetClassificationAccountsResult> Accounts { get; init; } = default!;
    }
}
