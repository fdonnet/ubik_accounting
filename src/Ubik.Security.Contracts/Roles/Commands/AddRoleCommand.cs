using System.ComponentModel.DataAnnotations;

namespace Ubik.Security.Contracts.Roles.Commands
{
    public record AddRoleCommand
    {
        [Required]
        [MaxLength(20)]
        public required string Code { get; init; }
        [Required]
        [MaxLength(100)]
        public required string Label { get; init; }
        [MaxLength(700)]
        public string? Description { get; init; }
    }
}
