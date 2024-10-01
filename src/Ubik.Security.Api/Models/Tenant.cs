using System.ComponentModel.DataAnnotations;
using Ubik.DB.Common;

namespace Ubik.Security.Api.Models
{
    public class Tenant : IConcurrencyCheckEntity, IAuditEntity
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        public required string Code { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Label { get; set; }
        [MaxLength(250)]
        public required string Description { get; set; }
        [Required]
        public Guid Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
