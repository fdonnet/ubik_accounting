using System.ComponentModel.DataAnnotations;

namespace Ubik.Security.Contracts.RoleAuthorizations.Commands
{
    public record AddRoleAuthorizationCommand
    {
        [Required]
        public Guid RoleId { get; init; }
        [Required]
        public Guid AuthorizationId { get; init; }
    }
}
