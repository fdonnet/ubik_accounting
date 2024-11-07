using Ubik.DB.Common;
using Ubik.DB.Common.Models;

namespace Ubik.Accounting.Structure.Api.Models
{
    public class AccountAccountGroup : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid AccountGroupId { get; set; }
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
