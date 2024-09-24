using Ubik.DB.Common;

namespace Ubik.Security.Api.Models
{
    public class UserRoleByTenant : IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public Guid UserTenantId { get; set; }
        public UserTenant? UserTenant { get; set; }
        public Guid RoleId { get; set; }
        public Role? Role { get; set; }
        public Guid Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
