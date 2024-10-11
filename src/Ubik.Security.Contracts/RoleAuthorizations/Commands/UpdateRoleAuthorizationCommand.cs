using System.ComponentModel.DataAnnotations;

namespace Ubik.Security.Contracts.RoleAuthorizations.Commands
{
    public record UpdateRoleAuthorizationCommand
    {
        [Required]
        public Guid Id { get; init; }
        [Required]
        public Guid RoleId { get; init; }
        [Required]
        public Guid AuthorizationId { get; init; }
        [Required]
        public Guid Version { get; init; }
    }
}
