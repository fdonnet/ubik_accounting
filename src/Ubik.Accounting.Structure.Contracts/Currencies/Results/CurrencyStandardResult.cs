namespace Ubik.Accounting.Structure.Contracts.Currencies.Results
{
    public record CurrencyStandardResult
    {
        public Guid Id { get; init; }
        public required string IsoCode { get; init; }
        public Guid Version { get; init; }
    }
}
