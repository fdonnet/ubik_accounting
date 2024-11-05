namespace Ubik.Accounting.Transaction.Api.Models
{
    public class AmountAdditionnalInfo
    {
        public decimal OriginalAmount { get; set; }
        public Guid OriginalCurrencyId { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
