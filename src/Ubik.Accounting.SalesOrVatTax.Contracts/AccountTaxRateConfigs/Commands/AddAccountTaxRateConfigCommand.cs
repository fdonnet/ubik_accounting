namespace Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Commands
{
    public record AddAccountTaxRateConfigCommand
    {
        public Guid AccountId { get; init; }
        public Guid TaxRateId { get; init; }
        public Guid TaxAccountId { get; init; }
    }
}
