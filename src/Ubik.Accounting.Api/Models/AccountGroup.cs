using System.ComponentModel.DataAnnotations;
using Ubik.DB.Common;

namespace Ubik.Accounting.Api.Models
{
    public class AccountGroup : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Label { get; set; }
        public string? Description { get; set; }
        public Guid ClassificationId { get; set; }
        public Classification? Classification { get; set; }
        public Guid? ParentAccountGroupId { get; set; }
        public AccountGroup? ParentAccountGroup { get; set; }
        public ICollection<AccountGroup>? ChildrenAccountGroups { get; set; }
        public ICollection<Account>? Accounts { get; set; }
        [ConcurrencyCheck]
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
