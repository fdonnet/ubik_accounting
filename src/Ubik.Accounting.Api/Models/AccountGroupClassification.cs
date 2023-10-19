using System.ComponentModel.DataAnnotations;

namespace Ubik.Accounting.Api.Models
{
    public class AccountGroupClassification
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Label { get; set; }
        public string? Description { get; set; }
        public ICollection<AccountGroup>? OwnedAccountGroups { get; set; }
        [ConcurrencyCheck]
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
