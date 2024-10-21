using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.RoleAuthorizations.Commands;
using Ubik.Security.Contracts.RoleAuthorizations.Events;

namespace Ubik.Security.Api.Mappers
{
    public static class RoleAuthorizationMappers
    {
        public static IEnumerable<RoleAuthorizationStandardResult> ToRoleAuthorizationStandardResults(this IEnumerable<RoleAuthorization> current)
        {
            return current.Select(x => new RoleAuthorizationStandardResult()
            {
                Id = x.Id,
                RoleId = x.RoleId,
                AuthorizationId = x.AuthorizationId,
                Version = x.Version,
            });
        }

        public static RoleAuthorization ToRoleAuthorization(this AddRoleAuthorizationCommand current)
        {
            return new RoleAuthorization
            {
                RoleId = current.RoleId,
                AuthorizationId = current.AuthorizationId,
            };
        }

        public static RoleAuthorization ToRoleAuthorization(this UpdateRoleAuthorizationCommand current)
        {
            return new RoleAuthorization
            {
                Id = current.Id,
                RoleId = current.RoleId,
                AuthorizationId = current.AuthorizationId,
                Version = current.Version,
            };
        }

        public static RoleAuthorizationAdded ToRoleAuthorizationAdded(this RoleAuthorization current)
        {
            return new RoleAuthorizationAdded()
            {
                Id = current.Id,
                RoleId = current.RoleId,
                AuthorizationId = current.AuthorizationId,
                Version = current.Version,
            };
        }

        public static RoleAuthorizationUpdated ToRoleAuthorizationUpdated(this RoleAuthorization current)
        {
            return new RoleAuthorizationUpdated()
            {
                Id = current.Id,
                RoleId = current.RoleId,
                AuthorizationId = current.AuthorizationId,
                Version = current.Version,
            };
        }

        public static RoleAuthorization ToRoleAuthorization(this RoleAuthorization forUpd, RoleAuthorization model)
        {
            model.Id = forUpd.Id;
            model.RoleId = forUpd.RoleId;
            model.AuthorizationId = forUpd.AuthorizationId;
            model.Version = forUpd.Version;

            return model;
        }

        public static RoleAuthorizationStandardResult ToRoleAuthorizationStandardResult(this RoleAuthorization current)
        {
            return new RoleAuthorizationStandardResult()
            {
                Id = current.Id,
                RoleId = current.RoleId,
                AuthorizationId = current.AuthorizationId,
                Version = current.Version,
            };
        }
    }
}
