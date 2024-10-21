using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Mappers;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Roles.Commands;
using Ubik.Security.Contracts.Roles.Events;

namespace Ubik.Security.Api.Features.Roles.Services
{
    public class RolesAdminCommandsService(SecurityDbContext ctx, IPublishEndpoint publishEndpoint) : IRolesAdminCommandsService
    {
        public async Task<Either<IServiceAndFeatureError, Role>> AddAsync(AddRoleCommand command)
        {
            return await ValidateIfNotAlreadyExistsAsync(command.ToRole())
                .BindAsync(AddInDbContextAsync)
                .BindAsync(AddSaveAndPublishAsync);
        }

        public async Task<Either<IServiceAndFeatureError, Role>> UpdateAsync(UpdateRoleCommand command)
        {
            var model = command.ToRole();

            return await GetAsync(model.Id)
                    .BindAsync(r => MapInDbContextAsync(r, model))
                    .BindAsync(ValidateIfNotAlreadyExistsWithOtherIdAsync)
                    .BindAsync(UpdateInDbContextAsync)
                    .BindAsync(UpdateSaveAndPublishAsync);
        }

        public async Task<Either<IServiceAndFeatureError, bool>> DeleteAsync(Guid id)
        {
            return await GetAsync(id)
                .BindAsync(DeleteInDbContextAsync)
                .BindAsync(DeleteSaveAndPublishAsync);
        }

        private async Task<Either<IServiceAndFeatureError, bool>> DeleteSaveAndPublishAsync(Role current)
        {
            await publishEndpoint.Publish(new RoleDeleted { Id = current.Id }, CancellationToken.None);
            await ctx.SaveChangesAsync();
            return true;
        }

        private async Task<Either<IServiceAndFeatureError, Role>> AddSaveAndPublishAsync(Role current)
        {
            await publishEndpoint.Publish(current.ToRoleAdded(), CancellationToken.None);
            await ctx.SaveChangesAsync();
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, Role>> UpdateSaveAndPublishAsync(Role current)
        {
            try
            {
                await publishEndpoint.Publish(current.ToRoleUpdated(), CancellationToken.None);
                await ctx.SaveChangesAsync();
                return current;
            }
            catch (UpdateDbConcurrencyException)
            {
                return new ResourceUpdateConcurrencyError("Role", current.Version.ToString());
            }
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

        private async Task<Either<IServiceAndFeatureError, Role>> MapInDbContextAsync
            (Role current, Role forUpdate)
        {
            current = forUpdate.ToRole(current);
            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, Role>> UpdateInDbContextAsync(Role current)
        {
            ctx.Entry(current).State = EntityState.Modified;
            ctx.SetAuditAndSpecialFields();

            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, Role>> AddInDbContextAsync(Role current)
        {
            current.Id = NewId.NextGuid();
            await ctx.Roles.AddAsync(current);
            ctx.SetAuditAndSpecialFields();
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, Role>> DeleteInDbContextAsync(Role current)
        {
            ctx.Entry(current).State = EntityState.Deleted;

            await Task.CompletedTask;
            return current;
        }
    }
}
