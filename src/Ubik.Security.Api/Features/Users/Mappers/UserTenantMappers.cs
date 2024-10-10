using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Tenants.Commands;
using Ubik.Security.Contracts.Tenants.Events;
using Ubik.Security.Contracts.Tenants.Results;
using Ubik.Security.Contracts.Users.Commands;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Security.Api.Features.Users.Mappers
{
    public static class UserTenantMappers
    {
        public static Tenant ToTenant(this AddTenantCommand current)
        {
            return new Tenant
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
            };
        }

        public static TenantStandardResult ToTenantStandardResult(this Tenant current)
        {
            return new TenantStandardResult
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version,
                Id = current.Id
            };
        }

        public static TenantAdded ToTenantAdded(this Tenant current)
        {
            return new TenantAdded()
            {
                Id = current.Id,
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version,
            };
        }
    }
}
