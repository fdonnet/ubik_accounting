namespace Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Results
{
    public record AccountTaxRateConfigStandardResult
    {
        public Guid Id { get; init; }
        public Guid AccountId { get; init; }
        public Guid TaxRateId { get; init; }
        public Guid TaxAccountId { get; init; }
        public Guid Version { get; init; }
    }
}
