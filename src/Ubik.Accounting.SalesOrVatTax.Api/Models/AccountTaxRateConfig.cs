using Ubik.DB.Common;
using Ubik.DB.Common.Models;

namespace Ubik.Accounting.SalesOrVatTax.Api.Models
{
    public class AccountTaxRateConfig : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid TaxRateId { get; set; }
        public Guid TaxAccountId { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
