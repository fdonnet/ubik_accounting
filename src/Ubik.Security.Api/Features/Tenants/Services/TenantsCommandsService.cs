using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Tenants.Commands;
using Ubik.Security.Contracts.Tenants.Events;
using Ubik.Security.Api.Mappers;

namespace Ubik.Security.Api.Features.Tenants.Services
{
    public class TenantsCommandsService(SecurityDbContext ctx, IPublishEndpoint publishEndpoint) : ITenantsCommandsService
    {
        public async Task<Either<IFeatureError, Tenant>> AddAsync(AddTenantCommand command)
        {
            return await ValidateIfNotAlreadyExistsAsync(command.ToTenant())
                    .BindAsync(AddInDbContextAsync)
                    .BindAsync(AddSaveAndPublishAsync);
        }

        public async Task<Either<IFeatureError, Tenant>> UpdateAsync(UpdateTenantCommand command)
        {
            var model = command.ToTenant();
            return await GetAsync(model.Id)
            .BindAsync(t => MapInDbContextAsync(t,model))
            .BindAsync(ValidateIfNotAlreadyExistsWithOtherIdAsync)
            .BindAsync(UpdateInDbContextAsync)
            .BindAsync(UpdateSaveAndPublishAsync);
        }

        //TODO: look at the deleted constrain and see if we want to expose that.
        public async Task<Either<IFeatureError, bool>> DeleteAsync(Guid id)
        {
            return await GetAsync(id)
                .BindAsync(DeleteInDbContextAsync)
                .BindAsync(DeleteSaveAndPublishAsync);
        }

        private async Task<Either<IFeatureError, bool>> DeleteSaveAndPublishAsync(Tenant current)
        {
            await publishEndpoint.Publish(new TenantDeleted { Id = current.Id }, CancellationToken.None);
            await ctx.SaveChangesAsync();
            return true;
        }

        private async Task<Either<IFeatureError, Tenant>> UpdateSaveAndPublishAsync(Tenant current)
        {
            try
            {
                await publishEndpoint.Publish(current.ToTenantUpdated(), CancellationToken.None);
                await ctx.SaveChangesAsync();
                return current;
            }
            catch (UpdateDbConcurrencyException)
            {
                return new ResourceUpdateConcurrencyError("Tenant", current.Version.ToString());
            }
        }

        private async Task<Either<IFeatureError, Tenant>> MapInDbContextAsync
            (Tenant current, Tenant forUpdate)
        {
            current = forUpdate.ToTenant(current);
            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IFeatureError, Tenant>> AddSaveAndPublishAsync(Tenant current)
        {
            await publishEndpoint.Publish(current.ToTenantAdded(), CancellationToken.None);
            await ctx.SaveChangesAsync();
            return current;
        }

        private async Task<Either<IFeatureError, Tenant>> GetAsync(Guid id)
        {
            var result = await ctx.Tenants.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("Tenant", "Id", id.ToString())
                : result;
        }

        private async Task<Either<IFeatureError, Tenant>> UpdateInDbContextAsync(Tenant current)
        {
            ctx.Entry(current).State = EntityState.Modified;
            ctx.SetAuditAndSpecialFields();

            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IFeatureError, Tenant>> AddInDbContextAsync(Tenant current)
        {
            current.Id = NewId.NextGuid();
            await ctx.Tenants.AddAsync(current);
            ctx.SetAuditAndSpecialFields();
            return current;
        }

        private async Task<Either<IFeatureError, Tenant>> ValidateIfNotAlreadyExistsAsync(Tenant current)
        {
            var exists = await ctx.Tenants.AnyAsync(a => a.Code == current.Code);
            return exists
                ? new ResourceAlreadyExistsError("Tenant", "Code", current.Code)
                : current;
        }

        private async Task<Either<IFeatureError, Tenant>> ValidateIfNotAlreadyExistsWithOtherIdAsync(Tenant current)
        {
            var exists = await ctx.Tenants.AnyAsync(a => a.Code == current.Code && a.Id != current.Id);

            return exists
                ? new ResourceAlreadyExistsError("Tenant", "Code", current.Code)
                : current;
        }

        private async Task<Either<IFeatureError, Tenant>> DeleteInDbContextAsync(Tenant current)
        {
            ctx.Entry(current).State = EntityState.Deleted;

            await Task.CompletedTask;
            return current;
        }
    }
}
