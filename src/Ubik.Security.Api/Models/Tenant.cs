using Ubik.DB.Common;
using Ubik.DB.Common.Models;

namespace Ubik.Security.Api.Models
{
    public class Tenant : IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Label { get; set; }
        public required string Description { get; set; }
        public bool IsActivated { get; set; } = true;
        public Guid Version { get; set; }
        public AuditData AuditInfo { get; set; } = default!;
    }
}
