namespace Ubik.Accounting.Contracts.Currencies.Results
{
    public record GetAllCurrenciesResult
    {
        public Guid Id { get; init; }
        public required string IsoCode { get; init; }
        public Guid Version { get; init; }
    }
}
