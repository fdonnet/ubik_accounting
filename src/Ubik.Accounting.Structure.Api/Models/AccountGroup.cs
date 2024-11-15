using System.ComponentModel.DataAnnotations;
using Ubik.DB.Common;
using Ubik.DB.Common.Models;

namespace Ubik.Accounting.Structure.Api.Models
{
    public class AccountGroup : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Label { get; set; }
        public string? Description { get; set; }
        public Guid ClassificationId { get; set; }
        public Guid? ParentAccountGroupId { get; set; }
        [ConcurrencyCheck]
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
