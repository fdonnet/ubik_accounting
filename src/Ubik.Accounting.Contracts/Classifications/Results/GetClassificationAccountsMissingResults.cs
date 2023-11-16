namespace Ubik.Accounting.Contracts.Classifications.Results
{
    public record GetClassificationAccountsMissingResults
    {
        public IEnumerable<GetClassificationAccountsMissingResult> Accounts { get; init; } = default!;
    }
}
