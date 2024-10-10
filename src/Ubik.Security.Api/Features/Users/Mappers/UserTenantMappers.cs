using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Tenants.Commands;
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

        public static TenantForUserResult ToUserForTenantResult(this Tenant current)
        {
            return new TenantForUserResult
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version,
                Id = current.Id
            };
        }

    }
}
