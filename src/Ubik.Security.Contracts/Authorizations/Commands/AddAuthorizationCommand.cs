using System.ComponentModel.DataAnnotations;

namespace Ubik.Security.Contracts.Authorizations.Commands
{
    public record AddAuthorizationCommand
    {
        [Required]
        [MaxLength(50)]
        public required string Code { get; init; }
        [Required]
        [MaxLength(100)]
        public required string Label { get; init; }
        [Required]
        [MaxLength(250)]
        public string? Description { get; init; }
        [Required]
        public bool IsOnlyForMegaAdmin { get; init; } = false;
    }
}
