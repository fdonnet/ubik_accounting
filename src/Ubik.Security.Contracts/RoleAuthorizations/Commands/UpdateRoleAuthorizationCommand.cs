using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
