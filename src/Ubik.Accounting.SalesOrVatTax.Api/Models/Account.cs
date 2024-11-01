using Ubik.DB.Common;

namespace Ubik.Accounting.SalesOrVatTax.Api.Models
{
    // Source of truth => Accounting.Structure.Api
    public class Account : ITenantEntity
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Label { get; set; }
        public bool Active { get; set; } = true;
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
    }
}
