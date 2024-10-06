﻿using LanguageExt;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Authorizations.Services
{
    public class AuthorizationsQueriesService(SecurityDbContext ctx) : IAuthorizationsQueriesService
    {
        public async Task<Either<IServiceAndFeatureError, Authorization>> GetAsync(Guid id)
        {
            var result = await ctx.Authorizations.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("Auhtorization", "Id", id.ToString())
                : result;
        }
    }
}