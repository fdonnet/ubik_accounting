using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Security.Contracts.RoleAuthorizations.Events
{
    public record RoleAuthorizationDeleted
    {
        public Guid Id { get; init; }
    }
}
