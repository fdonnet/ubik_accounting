using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubik.Security.Contracts.Tenants.Events
{
    public record TenantDeleted
    {
        public Guid Id { get; init; }
    }
}
