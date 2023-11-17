namespace Ubik.Accounting.Contracts.Classifications.Results
{
    public class GetClassificationStatusResult
    {
        public Guid Id { get; init; }
        public bool IsReady { get; init; }
        public IEnumerable<GetClassificationAccountsMissingResult> MissingAccounts { get; init; } = default!;
    }
}
