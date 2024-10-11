using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Tenants.Commands;
using Ubik.Security.Contracts.Tenants.Events;
using Ubik.Security.Api.Features.Mappers;

namespace Ubik.Security.Api.Features.Tenants.Services
{
    public class TenantsCommandsService(SecurityDbContext ctx, IPublishEndpoint publishEndpoint) : ITenantsCommandsService
    {
        public async Task<Either<IServiceAndFeatureError, Tenant>> AddAsync(AddTenantCommand command)
        {
            var result = await AddTenantAsync(command.ToTenant());

            return await result.MatchAsync<Either<IServiceAndFeatureError, Tenant>>(
            RightAsync: async r =>
            {
                await publishEndpoint.Publish(r.ToTenantAdded(), CancellationToken.None);
                await ctx.SaveChangesAsync();
                return r;
            },
            Left: err =>
            {
                return Prelude.Left(err);
            });
        }

        public async Task<Either<IServiceAndFeatureError, Tenant>> UpdateAsync(UpdateTenantCommand command)
        {
            var result = await UpdateTenantAsync(command.ToTenant());

            return await result.MatchAsync<Either<IServiceAndFeatureError, Tenant>>(
                RightAsync: async r =>
                {
                    try
                    {
                        //Store and publish AccountGroupAdded event
                        await publishEndpoint.Publish(r.ToTenantUpdated(), CancellationToken.None);
                        await ctx.SaveChangesAsync();
                        return r;
                    }
                    catch (UpdateDbConcurrencyException)
                    {
                        return new ResourceUpdateConcurrencyError("Tenant", r.Version.ToString());
                    }
                },
                Left: err =>
                {
                    return Prelude.Left(err);
                });
        }

        //TODO: look at the deleted constrain and see if we want to expose that.
        public async Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id)
        {
            var res = await ExecuteDeleteTenantAsync(id);

            return await res.MatchAsync<Either<IServiceAndFeatureError, bool>>(
                RightAsync: async r =>
                {
                    await publishEndpoint.Publish(new TenantDeleted { Id = id }, CancellationToken.None);
                    await ctx.SaveChangesAsync();
                    return true;
                },
                Left: err =>
                {
                    return Prelude.Left(err);
                });
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> GetAsync(Guid id)
        {
            var result = await ctx.Tenants.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("Tenant", "Id", id.ToString())
                : result;
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> UpdateTenantAsync(Tenant current)
        {
            return await GetAsync(current.Id).ToAsync()
               .Map(c => c = current.ToTenant(c))
               .Bind(c => ValidateIfNotAlreadyExistsWithOtherIdAsync(c).ToAsync())
               .Map(c =>
               {
                   ctx.Entry(c).State = EntityState.Modified;
                   ctx.SetAuditAndSpecialFields();
                   return c;
               });
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> AddTenantAsync(Tenant current)
        {
            return await ValidateIfNotAlreadyExistsAsync(current)
               .MapAsync(async ac =>
               {
                   ac.Id = NewId.NextGuid();
                   await ctx.Tenants.AddAsync(ac);
                   ctx.SetAuditAndSpecialFields();
                   return ac;
               });
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> ValidateIfNotAlreadyExistsAsync(Tenant current)
        {
            var exists = await ctx.Tenants.AnyAsync(a => a.Code == current.Code);
            return exists
                ? new ResourceAlreadyExistsError("Tenant", "Code", current.Code)
                : current;
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> ValidateIfNotAlreadyExistsWithOtherIdAsync(Tenant current)
        {
            var exists = await ctx.Tenants.AnyAsync(a => a.Code == current.Code && a.Id != current.Id);

            return exists
                ? new ResourceAlreadyExistsError("Tenant", "Code", current.Code)
                : current;
        }

        private async Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteTenantAsync(Guid id)
        {
            return await GetAsync(id)
                    .MapAsync(async ac =>
                    {
                        await ctx.Tenants.Where(x => x.Id == id).ExecuteDeleteAsync();
                        return true;
                    });
        }
    }
}
