using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Security.Contracts.Users.Events
{
    public record UserRoleAddedToTenant
    {
        public Guid UserId { get; init; }
        public Guid RoleId { get; init; }
        public Guid TenantId { get; init; }
    }
}
