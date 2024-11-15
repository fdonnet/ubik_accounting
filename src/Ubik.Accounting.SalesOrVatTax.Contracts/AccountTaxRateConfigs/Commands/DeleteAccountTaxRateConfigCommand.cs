namespace Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Commands
{
    public record DeleteAccountTaxRateConfigCommand
    {
        public Guid AccountId { get; init; }
        public Guid TaxRateId { get; init; }
    }
}
