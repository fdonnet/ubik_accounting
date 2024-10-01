using System.ComponentModel.DataAnnotations;
using Ubik.DB.Common;

namespace Ubik.Security.Api.Models
{
    public class User : IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Firstname { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Lastname { get; set; }
        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public required string Email { get; set; }
        public bool IsActivated { get; set; } = true;
        [Required]
        public Guid Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
