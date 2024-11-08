using Ubik.DB.Common;

namespace Ubik.Accounting.Transaction.Api.Models
{
    // Source of truth => Accounting.SalesOrVatTax
    public class TaxRate : ITenantEntity
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public decimal Rate { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
    }
}
