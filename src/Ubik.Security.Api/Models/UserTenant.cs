using Ubik.DB.Common;

namespace Ubik.Security.Api.Models
{
    public class UserTenant: IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }
        public Guid Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
