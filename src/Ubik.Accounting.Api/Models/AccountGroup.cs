using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ubik.DB.Common;

namespace Ubik.Accounting.Api.Models
{
    [Index(nameof(TenantId), IsUnique = false)]
    [Table("AccountGroups")]
    public class AccountGroup : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        [StringLength(20)]
        public required string Code { get; set; }
        [StringLength(100)]
        public required string Label { get; set; }
        [StringLength(700)]
        public string? Description { get; set; }
        public Guid? ParentAccountGroupId { get; set; }
        public AccountGroup? ParentAccountGroup { get; set; }
        public ICollection<AccountGroup>? ChildrenAccountGroups { get; set; }
        public ICollection<Account>? Accounts { get; set; }
        [ConcurrencyCheck]
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required Guid CreatedBy { get; set; }
        public User CreatedByUser { get; set; } = default!;
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
        public User? ModifiedByUser { get; set; }
    }
}
