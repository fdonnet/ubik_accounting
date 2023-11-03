namespace Ubik.Accounting.Contracts.Classifications.Results
{
    public record GetAllClassificationsResults
    {
        public IEnumerable<GetAllClassificationsResult> Classifications { get; init; } = default!;
    }
}
