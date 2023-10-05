using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using Ubik.DB.Common;

namespace Ubik.Accounting.Api.Models
{
    [Index(nameof(Code), IsUnique = true)]
    [Index(nameof(TenantId), IsUnique = false)]
    public class Account : ITenantEntity, IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        [StringLength(20)]
        public required string Code { get; set; }
        [StringLength(100)]
        public required string Label { get; set; }
        [StringLength(700)]
        public  string? Description { get; set; }
        public Guid AccountGroupId { get; set; }
        public AccountGroup? Group { get; set; }
        [ConcurrencyCheck]
        public Guid Version { get; set; }
        public Guid TenantId { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        public User CreatedByUser { get; set; } = default!;
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
        public User? ModifiedByUser { get; set; }
    }
}
