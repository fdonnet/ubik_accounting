using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Security.Contracts.Roles.Events
{
    public record RoleDeleted
    {
        public Guid Id { get; init; }
    }
}
