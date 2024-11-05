using Ubik.DB.Common;

namespace Ubik.Accounting.Transaction.Api.Models
{
    public class Tx : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required DateOnly ValueDate { get; set; }
        public required string Label { get; set; }
        public decimal Amount { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
