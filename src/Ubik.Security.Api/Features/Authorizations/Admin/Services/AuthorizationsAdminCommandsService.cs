using LanguageExt;
using MassTransit;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Authorizations.Mappers;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Authorizations.Commands;
using Ubik.Security.Contracts.Authorizations.Events;


namespace Ubik.Security.Api.Features.Authorizations.Admin.Services
{
    public class AuthorizationsAdminCommandsService(SecurityDbContext ctx, IPublishEndpoint publishEndpoint) : IAuthorizationsAdminCommandsService
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

        public async Task<Either<IServiceAndFeatureError, Authorization>> UpdateAsync(UpdateAuthorizationCommand authorizationCommand)
        {
            var result = await UpdateAuthorizationAsync(authorizationCommand.ToAuthorization());

            return await result.MatchAsync<Either<IServiceAndFeatureError, Authorization>>(
                RightAsync: async r =>
                {
                    try
                    {
                        //Store and publish AccountGroupAdded event
                        await publishEndpoint.Publish(r.ToAuthorizationUpdated(), CancellationToken.None);
                        await ctx.SaveChangesAsync();
                        return r;
                    }
                    catch (UpdateDbConcurrencyException)
                    {
                        return new ResourceUpdateConcurrencyError("Authorization", r.Version.ToString());
                    }
                },
                Left: err =>
                {
                    return Prelude.Left(err);
                });
        }


        public async Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id)
        {
            var res = await ExecuteDeleteAuthorizationAsync(id);

            return await res.MatchAsync<Either<IServiceAndFeatureError, bool>>(
                RightAsync: async r =>
                {
                    await publishEndpoint.Publish(new AuthorizationDeleted { Id = id }, CancellationToken.None);
                    await ctx.SaveChangesAsync();
                    return true;
                },
                Left: err =>
                {
                    return Prelude.Left(err);
                });
        }

        private async Task<Either<IServiceAndFeatureError, Authorization>> UpdateAuthorizationAsync(Authorization authorization)
        {
            return await GetAsync(authorization.Id).ToAsync()
               .Map(c => c = authorization.ToAuthorization(c))
               .Bind(c => ValidateIfNotAlreadyExistsWithOtherIdAsync(c).ToAsync())
               .Map(c =>
               {
                   ctx.Entry(c).State = EntityState.Modified;
                   ctx.SetAuditAndSpecialFields();
                   return c;
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

        public async Task<Either<IServiceAndFeatureError, Authorization>> GetAsync(Guid id)
        {
            var result = await ctx.Authorizations.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("Auhtorization", "Id", id.ToString())
                : result;
        }

        private async Task<Either<IServiceAndFeatureError, Authorization>> ValidateIfNotAlreadyExistsAsync(Authorization auth)
        {
            var exists = await ctx.Authorizations.AnyAsync(a => a.Code == auth.Code);
            return exists
                ? new ResourceAlreadyExistsError("Authorization", "Code", auth.Code)
                : auth;
        }

        private async Task<Either<IServiceAndFeatureError, Authorization>> ValidateIfNotAlreadyExistsWithOtherIdAsync(Authorization auth)
        {
            var exists = await ctx.Authorizations.AnyAsync(a => a.Code == auth.Code && a.Id != auth.Id);

            return exists
                ? new ResourceAlreadyExistsError("Authorization", "Code", auth.Code)
                : auth;
        }

        private async Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAuthorizationAsync(Guid id)
        {
            return await GetAsync(id).ToAsync()
                    .MapAsync(async ac =>
                    {
                        await ctx.Roles.Where(x => x.Id == id).ExecuteDeleteAsync();
                        return true;
                    });
        }
    }
}
