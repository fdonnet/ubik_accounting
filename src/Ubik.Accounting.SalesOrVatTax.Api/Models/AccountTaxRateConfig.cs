namespace Ubik.Accounting.SalesOrVatTax.Api.Models
{
    public class AccountTaxRateConfig
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }
        public Guid TaxRateId { get; set; }
        public TaxRate? TaxRate { get; set; }
        public Guid TaxAccountId { get; set; }
        public Account? TaxAccount { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
