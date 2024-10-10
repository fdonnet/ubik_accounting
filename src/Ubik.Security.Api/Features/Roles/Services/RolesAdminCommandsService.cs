using LanguageExt;
using MassTransit;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Authorizations.Mappers;
using Ubik.Security.Api.Features.Roles.Mappers;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Authorizations.Commands;
using Ubik.Security.Contracts.Authorizations.Events;
using Ubik.Security.Contracts.RoleAuthorizations.Commands;
using Ubik.Security.Contracts.Roles.Commands;
using Ubik.Security.Contracts.Roles.Events;

namespace Ubik.Security.Api.Features.Roles.Services
{
    public class RolesAdminCommandsService(SecurityDbContext ctx, IPublishEndpoint publishEndpoint) : IRolesAdminCommandsService
    {
        public async Task<Either<IServiceAndFeatureError, Role>> AddAsync(AddRoleCommand command)
        {
            var result = await AddRoleAsync(command.ToRole());

            return await result.MatchAsync<Either<IServiceAndFeatureError, Role>>(
            RightAsync: async r =>
            {
                await publishEndpoint.Publish(r.ToRoleAdded(), CancellationToken.None);
                await ctx.SaveChangesAsync();
                return r;
            },
            Left: err =>
            {
                return Prelude.Left(err);
            });
        }

        public async Task<Either<IServiceAndFeatureError, Role>> UpdateAsync(UpdateRoleCommand command)
        {
            var result = await UpdateRoleAsync(command.ToRole());

            return await result.MatchAsync<Either<IServiceAndFeatureError, Role>>(
                RightAsync: async r =>
                {
                    try
                    {
                        //Store and publish AccountGroupAdded event
                        await publishEndpoint.Publish(r.ToRoleUpdated(), CancellationToken.None);
                        await ctx.SaveChangesAsync();
                        return r;
                    }
                    catch (UpdateDbConcurrencyException)
                    {
                        return new ResourceUpdateConcurrencyError("Role", r.Version.ToString());
                    }
                },
                Left: err =>
                {
                    return Prelude.Left(err);
                });
        }

        public async Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id)
        {
            var res = await ExecuteDeleteRoleAsync(id);

            return await res.MatchAsync<Either<IServiceAndFeatureError, bool>>(
                RightAsync: async r =>
                {
                    await publishEndpoint.Publish(new RoleDeleted { Id = id }, CancellationToken.None);
                    await ctx.SaveChangesAsync();
                    return true;
                },
                Left: err =>
                {
                    return Prelude.Left(err);
                });
        }

        private async Task<Either<IServiceAndFeatureError, Role>> GetAsync(Guid id)
        {
            var result = await ctx.Roles.FirstOrDefaultAsync(r => r.Id == id && r.TenantId == null);

            return result == null
                ? new ResourceNotFoundError("Role", "Id", id.ToString())
                : result;
        }

        private async Task<Either<IServiceAndFeatureError, Role>> ValidateIfNotAlreadyExistsAsync(Role current)
        {
            var exists = await ctx.Roles.AnyAsync(a => a.Code == current.Code && a.TenantId == null);
            return exists
                ? new ResourceAlreadyExistsError("Role", "Code", current.Code)
                : current;
        }

        private async Task<Either<IServiceAndFeatureError, Role>> ValidateIfNotAlreadyExistsWithOtherIdAsync(Role current)
        {
            var exists = await ctx.Roles.AnyAsync(a => a.Code == current.Code && a.Id != current.Id && a.TenantId == null);

            return exists
                ? new ResourceAlreadyExistsError("Role", "Code", current.Code)
                : current;
        }

        private async Task<Either<IServiceAndFeatureError, Role>> UpdateRoleAsync(Role current)
        {
            return await GetAsync(current.Id).ToAsync()
               .Map(c => c = current.ToRole(c))
               .Bind(c => ValidateIfNotAlreadyExistsWithOtherIdAsync(c).ToAsync())
               .Map(c =>
               {
                   ctx.Entry(c).State = EntityState.Modified;
                   ctx.SetAuditAndSpecialFieldsForAdmin();
                   return c;
               });
        }

        private async Task<Either<IServiceAndFeatureError, Role>> AddRoleAsync(Role current)
        {
            return await ValidateIfNotAlreadyExistsAsync(current)
               .MapAsync(async ac =>
               {
                   ac.Id = NewId.NextGuid();
                   await ctx.Roles.AddAsync(ac);
                   ctx.SetAuditAndSpecialFieldsForAdmin();
                   return ac;
               });
        }

        private async Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteRoleAsync(Guid id)
        {
            return await GetAsync(id)
                    .MapAsync(async ac =>
                    {
                        await ctx.Roles.Where(x => x.Id == id).ExecuteDeleteAsync();
                        return true;
                    });
        }
    }
}
