using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Authorizations.Mappers;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Authorizations.Commands;
using Ubik.Security.Contracts.Authorizations.Results;


namespace Ubik.Security.Api.Features.Authorizations.Services
{
    public class AuthorizationsCommandsService(SecurityDbContext ctx, IPublishEndpoint publishEndpoint) : IAuthorizationsCommandsService
    {

        public async Task<Either<IServiceAndFeatureError, Authorization>> AddAsync(AddAuthorizationCommand authorizationCommand)
        {
            var result = await AddAuthorizationAsync(authorizationCommand.ToAuthorization());

            return await result.MatchAsync<Either<IServiceAndFeatureError, Authorization>>(
            RightAsync: async r =>
            {
                await publishEndpoint.Publish(r.ToAuthorizationAdded(), CancellationToken.None);
                await ctx.SaveChangesAsync();
                return r;
            },
            Left: err =>
            {
                return Prelude.Left(err);
            });
        }

        private async Task<Either<IServiceAndFeatureError, Authorization>> AddAuthorizationAsync(Authorization authorization)
        {
            return await ValidateIfNotAlreadyExistsAsync(authorization).ToAsync()
               .MapAsync(async ac =>
               {
                   ac.Id = NewId.NextGuid();
                   await ctx.Authorizations.AddAsync(ac);
                   ctx.SetAuditAndSpecialFields();
                   return ac;
               });
        }

        private async Task<Either<IServiceAndFeatureError, Authorization>> ValidateIfNotAlreadyExistsAsync(Authorization auth)
        {
            var exists = await ctx.Authorizations.AnyAsync(a => a.Code == auth.Code);
            return exists
                ? new ResourceAlreadyExistsError("Authorization", "Code", auth.Code)
                : auth;
        }
    }
}
