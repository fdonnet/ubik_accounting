using System.ComponentModel.DataAnnotations;

namespace Ubik.Security.Contracts.Authorizations.Commands
{
    public record AddAuthorizationCommand
    {
        [Required]
        [MaxLength(50)]
        public required String Code { get; init; }
        [Required]
        [MaxLength(100)]
        public required String Label { get; init; }
        [MaxLength(700)]
        public required String Description { get; init; }
        [Required]
        public Boolean IsOnlyForMegaAdmin { get; init; }
    }
}
