using Ubik.DB.Common;

namespace Ubik.Accounting.Structure.Api.Models
{
    public class AccountVatConfig : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }
        public Guid VatRateId { get; set; }
        public VatRate? VatRate { get; set; }
        public Guid VatAccountId { get; set; }
        public Account? VatAccount { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
