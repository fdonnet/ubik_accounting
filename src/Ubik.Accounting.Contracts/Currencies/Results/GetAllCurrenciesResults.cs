namespace Ubik.Accounting.Contracts.Currencies.Results
{
    public record GetAllCurrenciesResults
    {
        public IEnumerable<GetAllCurrenciesResult> Currencies { get; init; } = default!;
    }
}
