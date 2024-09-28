﻿using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Authorizations.Commands;
using Ubik.Security.Contracts.Authorizations.Events;
using Ubik.Security.Contracts.Authorizations.Results;

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
        public static AuthorizationAdded ToAuthorizationAdded(this Authorization current)
        {
            return new AuthorizationAdded()
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                IsOnlyForMegaAdmin = current.IsOnlyForMegaAdmin,
                Version = current.Version,
                Id = current.Id,
            };
        }

        public static AddAuthorizationResult ToAddAuthorizationResult(this Authorization current)
        {
            return new AddAuthorizationResult()
            {
                Code = current.Code,
                Label = current.Label,
                Description = current.Description,
                IsOnlyForMegaAdmin = current.IsOnlyForMegaAdmin,
                Version = current.Version,
                Id = current.Id
            };
        }
    }
}
