using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Security.Contracts.Authorizations.Events
{
    public record AuthorizationDeleted
    {
        public Guid Id { get; init; }
    }
}
