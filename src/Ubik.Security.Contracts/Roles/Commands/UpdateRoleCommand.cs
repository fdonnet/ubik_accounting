using System.ComponentModel.DataAnnotations;

namespace Ubik.Security.Contracts.Roles.Commands
{
    public record UpdateRoleCommand
    {
        [Required]
        public Guid Id { get; init; }
        [Required]
        [MaxLength(50)]
        public required string Code { get; init; }
        [Required]
        [MaxLength(100)]
        public required string Label { get; init; }
        [MaxLength(700)]
        public string? Description { get; init; }
        [Required]
        public Guid Version { get; init; }
    }
}
