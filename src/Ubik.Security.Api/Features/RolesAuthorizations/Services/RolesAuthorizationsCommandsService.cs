using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Mappers;
using Ubik.Security.Api.Features.RolesAuthorizations.Errors;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.RoleAuthorizations.Commands;
using Ubik.Security.Contracts.RoleAuthorizations.Events;

namespace Ubik.Security.Api.Features.RolesAuthorizations.Services
{
    public class RolesAuthorizationsCommandsService(SecurityDbContext ctx, IPublishEndpoint publishEndpoint) : IRolesAuthorizationsCommandsService
    {
        public async Task<Either<IServiceAndFeatureError, RoleAuthorization>> AddAsync(AddRoleAuthorizationCommand command)
        {
            var result = await AddRoleAuthorizationAsync(command.ToRoleAuthorization());

            return await result.MatchAsync<Either<IServiceAndFeatureError, RoleAuthorization>>(
            RightAsync: async r =>
            {
                await publishEndpoint.Publish(r.ToRoleAuthorizationAdded(), CancellationToken.None);
                await ctx.SaveChangesAsync();
                return r;
            },
            Left: err =>
            {
                return Prelude.Left(err);
            });
        }

        public async Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id)
        {
            var res = await ExecuteDeleteRoleAuthorizationAsync(id);

            return await res.MatchAsync<Either<IServiceAndFeatureError, bool>>(
                RightAsync: async r =>
                {
                    await publishEndpoint.Publish(new RoleAuthorizationDeleted { Id = id }, CancellationToken.None);
                    await ctx.SaveChangesAsync();
                    return true;
                },
                Left: err =>
                {
                    return Prelude.Left(err);
                });
        }

        public async Task<Either<IServiceAndFeatureError, RoleAuthorization>> GetAsync(Guid id)
        {
            var result = await ctx.RolesAuthorizations.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("RoleAuthorization", "Id", id.ToString())
                : result;
        }

        private async Task<Either<IServiceAndFeatureError, RoleAuthorization>> ValidateIfForBaseRoleAsync(RoleAuthorization current)
        {
            var exists = await ctx.Roles.AnyAsync(r => r.Id == current.RoleId && r.TenantId == null);

            return exists
                ? current
                : new RoleAuthorizationIsNotABaseRoleError(current.RoleId);
        }

        private async Task<Either<IServiceAndFeatureError, RoleAuthorization>> ValidateIfAuhtorizationAsync(RoleAuthorization current)
        {
            var exists = await ctx.Authorizations.FindAsync(current.AuthorizationId) != null;

            return exists
                ? current
                : new BadParamExternalResourceNotFound("RoleAuthorization","Authorization","AuthorizationId", current.AuthorizationId.ToString());
        }

        private async Task<Either<IServiceAndFeatureError, RoleAuthorization>> ValidateIfNotAlreadyExistsAsync(RoleAuthorization current)
        {
            var exists = await ctx.RolesAuthorizations.AnyAsync(a => a.RoleId == current.RoleId
                                                                && a.AuthorizationId == current.AuthorizationId);
            return exists
                ? new ResourceAlreadyExistsError("RoleAuthorization", "RoleId and AuthorizationId",
                    current.RoleId.ToString() + current.AuthorizationId.ToString())
                : current;
        }

        private async Task<Either<IServiceAndFeatureError, RoleAuthorization>> ValidateIfNotAlreadyExistsWithOtherIdAsync(RoleAuthorization current)
        {
            var exists = await ctx.RolesAuthorizations.AnyAsync(a => a.RoleId == current.RoleId
                                                                && a.AuthorizationId == current.AuthorizationId
                                                                && a.Id != current.Id);

            return exists
                ? new ResourceAlreadyExistsError("RoleAuthorization", "RoleId and AuthorizationId", current.RoleId.ToString() + current.AuthorizationId.ToString())
                : current;
        }

        private async Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteRoleAuthorizationAsync(Guid id)
        {
            return await GetAsync(id)
                    .BindAsync(notexists => ValidateIfForBaseRoleAsync(notexists))
                    .MapAsync(async ac =>
                    {
                        await ctx.RolesAuthorizations.Where(x => x.Id == id).ExecuteDeleteAsync();
                        return true;
                    });
        }

        private async Task<Either<IServiceAndFeatureError, RoleAuthorization>> AddRoleAuthorizationAsync(RoleAuthorization current)
        {
            return await ValidateIfNotAlreadyExistsAsync(current)
                .BindAsync(ra => ValidateIfForBaseRoleAsync(ra))
                .BindAsync(ra => ValidateIfAuhtorizationAsync(ra))
               .MapAsync(async ac =>
               {
                   ac.Id = NewId.NextGuid();
                   await ctx.RolesAuthorizations.AddAsync(ac);
                   ctx.SetAuditAndSpecialFieldsForAdmin();
                   return ac;
               });
        }
    }
}
