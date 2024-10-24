using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.RolesAuthorizations.Errors;
using Ubik.Security.Api.Mappers;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.RoleAuthorizations.Commands;
using Ubik.Security.Contracts.RoleAuthorizations.Events;
using Ubik.Security.Contracts.Roles.Events;

namespace Ubik.Security.Api.Features.RolesAuthorizations.Services
{
    public class RolesAuthorizationsCommandsService(SecurityDbContext ctx, IPublishEndpoint publishEndpoint) : IRolesAuthorizationsCommandsService
    {
        public async Task<Either<IServiceAndFeatureError, RoleAuthorization>> AddAsync(AddRoleAuthorizationCommand command)
        {
            return await ValidateIfNotAlreadyExistsAsync(command.ToRoleAuthorization())
                .BindAsync(ValidateIfForBaseRoleAsync)
                .BindAsync(ValidateIfAuhtorizationAsync)
                .BindAsync(AddInDbContextAsync)
                .BindAsync(AddSaveAndPublishAsync);
        }

        public async Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id)
        {
            return await GetAsync(id)
                .BindAsync(ValidateIfForBaseRoleAsync)
                .BindAsync(DeleteInDbContextAsync)
                .BindAsync(DeleteSaveAndPublishAsync);
        }

        private async Task<Either<IServiceAndFeatureError, RoleAuthorization>> GetAsync(Guid id)
        {
            var result = await ctx.RolesAuthorizations.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("RoleAuthorization", "Id", id.ToString())
                : result;
        }

        private async Task<Either<IServiceAndFeatureError, bool>> DeleteSaveAndPublishAsync(RoleAuthorization current)
        {
            await publishEndpoint.Publish(new RoleAuthorizationDeleted { Id = current.Id }, CancellationToken.None);
            await ctx.SaveChangesAsync();
            return true;
        }

        private async Task<Either<IServiceAndFeatureError, RoleAuthorization>> AddSaveAndPublishAsync(RoleAuthorization current)
        {
            await publishEndpoint.Publish(current.ToRoleAuthorizationAdded(), CancellationToken.None);
            await ctx.SaveChangesAsync();
            return current;
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

        private async Task<Either<IServiceAndFeatureError, RoleAuthorization>> DeleteInDbContextAsync(RoleAuthorization current)
        {
            ctx.Entry(current).State = EntityState.Deleted;

            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, RoleAuthorization>> AddInDbContextAsync(RoleAuthorization current)
        {
            current.Id = NewId.NextGuid();
            await ctx.RolesAuthorizations.AddAsync(current);
            ctx.SetAuditAndSpecialFields();
            return current;
        }
    }
}
