using System.ComponentModel.DataAnnotations;
using Ubik.DB.Common;

namespace Ubik.Security.Api.Models
{
    public class Authorization : IConcurrencyCheckEntity, IAuditEntity
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        public required string Code { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Label { get; set; }
        [MaxLength(700)]
        public string? Description { get; set; }
        [Required]
        public bool IsOnlyForMegaAdmin { get; set; } = false;
        [Required]
        public Guid Version { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
