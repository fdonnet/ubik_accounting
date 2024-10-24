using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Authorizations.Commands;
using Ubik.Security.Contracts.Authorizations.Events;
using Ubik.Security.Contracts.Authorizations.Results;

namespace Ubik.Security.Api.Mappers
{
    public static class AuthorizationMappers
    {
        public static IEnumerable<AuthorizationStandardResult> ToAuthorizationStandardResults(this IEnumerable<Authorization> current)
        {
            return current.Select(x => new AuthorizationStandardResult()
            {
                Id = x.Id,
                Code = x.Code,
                Label = x.Label,
                Description = x.Description,
                Version = x.Version
            });
        }
        public static Authorization ToAuthorization(this AddAuthorizationCommand current)
        {
            return new Authorization
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
            };
        }

        public static Authorization ToAuthorization(this UpdateAuthorizationCommand current)
        {
            return new Authorization
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version,
                Id = current.Id
            };
        }
        public static AuthorizationAdded ToAuthorizationAdded(this Authorization current)
        {
            return new AuthorizationAdded()
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version,
                Id = current.Id,
            };
        }

        public static AuthorizationUpdated ToAuthorizationUpdated(this Authorization current)
        {
            return new AuthorizationUpdated()
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                Version = current.Version,
                Id = current.Id,
            };
        }

        public static Authorization ToAuthorization(this Authorization forUpd, Authorization model)
        {
            model.Id = forUpd.Id;
            model.Code = forUpd.Code;
            model.Label = forUpd.Label;
            model.Description = forUpd.Description;
            model.Version = forUpd.Version;

            return model;
        }

        public static AuthorizationStandardResult ToAuthorizationStandardResult(this Authorization current)
        {
            return new AuthorizationStandardResult()
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
