using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Mappers;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Authorizations.Commands;
using Ubik.Security.Contracts.Authorizations.Events;


namespace Ubik.Security.Api.Features.Authorizations.Services
{
    public class AuthorizationsCommandsService(SecurityDbContext ctx, IPublishEndpoint publishEndpoint) : IAuthorizationsCommandsService
    {

        public async Task<Either<IServiceAndFeatureError, Authorization>> AddAsync(AddAuthorizationCommand command)
        {
            return await ValidateIfNotAlreadyExistsAsync(command.ToAuthorization())
                .BindAsync(AddInDbContextAsync)
                .BindAsync(AddSaveAndPublishAsync);
        }

        public async Task<Either<IServiceAndFeatureError, Authorization>> UpdateAsync(UpdateAuthorizationCommand command)
        {
            var model = command.ToAuthorization();

            return await GetAsync(model.Id)
               .BindAsync(a => MapInDbContextAsync(a, model))
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

        private async Task<Either<IServiceAndFeatureError, bool>> DeleteSaveAndPublishAsync(Authorization current)
        {
            await publishEndpoint.Publish(new AuthorizationDeleted { Id = current.Id }, CancellationToken.None);
            await ctx.SaveChangesAsync();
            return true;
        }

        private async Task<Either<IServiceAndFeatureError, Authorization>> MapInDbContextAsync
            (Authorization current, Authorization forUpdate)
        {
            current = forUpdate.ToAuthorization(current);
            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, Authorization>> AddSaveAndPublishAsync(Authorization authorization)
        {
            await publishEndpoint.Publish(authorization.ToAuthorizationAdded(), CancellationToken.None);
            await ctx.SaveChangesAsync();
            return authorization;
        }

        private async Task<Either<IServiceAndFeatureError, Authorization>> UpdateSaveAndPublishAsync(Authorization authorization)
        {
            try
            {
                await publishEndpoint.Publish(authorization.ToAuthorizationUpdated(), CancellationToken.None);
                await ctx.SaveChangesAsync();
                return authorization;
            }
            catch (UpdateDbConcurrencyException)
            {
                return new ResourceUpdateConcurrencyError("Authorization", authorization.Version.ToString());
            }
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

        private async Task<Either<IServiceAndFeatureError, Authorization>> GetAsync(Guid id)
        {
            var result = await ctx.Authorizations.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("Authorization", "Id", id.ToString())
                : result;
        }

        private async Task<Either<IServiceAndFeatureError, Authorization>> UpdateInDbContextAsync(Authorization authorization)
        {
            ctx.Entry(authorization).State = EntityState.Modified;
            ctx.SetAuditAndSpecialFields();

            await Task.CompletedTask;
            return authorization;
        }

        private async Task<Either<IServiceAndFeatureError, Authorization>> AddInDbContextAsync(Authorization authorization)
        {
            authorization.Id = NewId.NextGuid();
            await ctx.Authorizations.AddAsync(authorization);
            ctx.SetAuditAndSpecialFields();
            return authorization;
        }

        private async Task<Either<IServiceAndFeatureError, Authorization>> DeleteInDbContextAsync(Authorization authorization)
        {
            ctx.Entry(authorization).State = EntityState.Deleted;

            await Task.CompletedTask;
            return authorization;
        }
    }
}
