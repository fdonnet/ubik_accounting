using Microsoft.AspNetCore.Http.HttpResults;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Authorizations.Commands;

namespace Ubik.Security.Api.Features.Authorizations.Mappers
{
    public static class AuthorizationMappers
    {
        public static Authorization ToAuthorization(this AddAuthorizationCommand current)
        {
            return new Authorization
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                IsOnlyForMegaAdmin = current.IsOnlyForMegaAdmin,
            };
        }
    }
}
