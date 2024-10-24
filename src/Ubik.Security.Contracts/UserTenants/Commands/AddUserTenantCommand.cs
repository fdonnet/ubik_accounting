using System.ComponentModel.DataAnnotations;

namespace Ubik.Security.Contracts.UserTenants.Commands
{
    public record AddUserTenantCommand
    {
        [Required]
        public Guid UserId { get; init; }
    }
}