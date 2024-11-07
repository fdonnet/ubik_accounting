using Ubik.DB.Common;
using Ubik.DB.Common.Models;

namespace Ubik.Security.Api.Models
{
    public class UserTenant: IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public Guid Version { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
