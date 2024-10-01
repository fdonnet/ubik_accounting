using System.ComponentModel.DataAnnotations;
using Ubik.DB.Common;

namespace Ubik.Security.Api.Models
{
    public class Authorization : IConcurrencyCheckEntity, IAuditEntity
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Label { get; set; }
        public string? Description { get; set; }
        public bool IsOnlyForMegaAdmin { get; set; } = false;
        public Guid Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedBy { get; set; }
    }
}
