using Ubik.Accounting.Structure.Contracts.Accounts.Enums;
using Ubik.DB.Common;
using Ubik.DB.Common.Models;

namespace Ubik.Accounting.Structure.Api.Models
{
    public class Account : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Label { get; set; }
        public required Guid CurrencyId { get; set; }
        public  string? Description { get; set; }
        public AccountCategory Category { get; set; }
        public AccountDomain Domain { get; set; }
        public bool Active { get; set; } = true;
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
