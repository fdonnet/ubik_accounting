using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Roles.Commands;
using Ubik.Security.Contracts.Roles.Events;
using Ubik.Security.Contracts.Roles.Results;

namespace Ubik.Security.Api.Mappers
{
    public static class RoleMappers
    {
        public static IEnumerable<RoleStandardResult> ToRoleStandardResults(this IEnumerable<Role> current)
        {
            return current.Select(x => new RoleStandardResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                Version = x.Version,
            });
        }

        public static RoleStandardResult ToRoleStandardResult(this Role current)
        {
            return new RoleStandardResult()
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version,
                Id = current.Id
            };
        }

        public static Role ToRole(this Role forUpd, Role model)
        {
            model.Id = forUpd.Id;
            model.Code = forUpd.Code;
            model.Label = forUpd.Label;
            model.Description = forUpd.Description;
            model.Version = forUpd.Version;
            model.TenantId = forUpd.TenantId;

            return model;
        }

        //Need to check for tenant role
        public static Role ToRole(this AddRoleCommand current)
        {
            return new Role
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
            };
        }
        public static Role ToRole(this UpdateRoleCommand current)
        {
            return new Role
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Id = current.Id,
                Version = current.Version,
            };
        }

        public static RoleAdded ToRoleAdded(this Role current)
        {
            return new RoleAdded()
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version,
                Id = current.Id,
                TenantId = current.TenantId
            };
        }

        public static RoleUpdated ToRoleUpdated(this Role current)
        {
            return new RoleUpdated()
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version,
                Id = current.Id,
                TenantId = current.TenantId
            };
        }
    }
}
