using Ubik.DB.Common;

namespace Ubik.Accounting.Api.Models
{
    public class Account : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Label { get; set; }
        public  string? Description { get; set; }
        public Guid AccountGroupId { get; set; }
        public AccountGroup? AccountGroup { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public User CreatedByUser { get; set; } = default!;
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
        public User? ModifiedByUser { get; set; }
    }
}
