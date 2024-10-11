using System.ComponentModel.DataAnnotations;

namespace Ubik.Security.Contracts.Tenants.Commands
{
    public record UpdateTenantCommand
    {
        [Required]
        public Guid Id { get; init; }
        [Required]
        [MaxLength(50)]
        public required string Code { get; init; }
        [Required]
        [MaxLength(100)]
        public required string Label { get; init; }
        [Required]
        [MaxLength(250)]
        public required string Description { get; init; }
        [Required]
        public Guid Version { get; init; }
    }
}
