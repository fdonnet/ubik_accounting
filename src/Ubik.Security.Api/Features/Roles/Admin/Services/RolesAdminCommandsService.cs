using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Authorizations.Mappers;
using Ubik.Security.Api.Features.Roles.Mappers;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Authorizations.Commands;
using Ubik.Security.Contracts.RoleAuthorizations.Commands;
using Ubik.Security.Contracts.Roles.Commands;

namespace Ubik.Security.Api.Features.Roles.Admin.Services
{
    public class RolesAdminCommandsService(SecurityDbContext ctx) : IRolesAdminCommandsService
    {
        public Task<Either<IServiceAndFeatureError, Role>> AddAsync(AddRoleCommand authorizationCommand)
        {
            throw new NotImplementedException();
        }

        public Task<Either<IServiceAndFeatureError, Role>> UpdateAsync(UpdateRoleCommand authorizationCommand)
        {
            throw new NotImplementedException();
        }

        public async Task<Either<IServiceAndFeatureError, Role>> GetAsync(Guid id)
        {
            var result = await ctx.Roles.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("Role", "Id", id.ToString())
                : result;
        }

        private async Task<Either<IServiceAndFeatureError, Role>> UpdateRoleAsync(Role current)
        {
            return await GetAsync(current.Id).ToAsync()
               .Map(c => c = current.ToRole(c))
               .Bind(c => ValidateIfNotAlreadyExistsWithOtherIdAsync(c).ToAsync())
               .Map(c =>
               {
                   ctx.Entry(c).State = EntityState.Modified;
                   ctx.SetAuditAndSpecialFields();
                   return c;
               });
        }

        private async Task<Either<IServiceAndFeatureError, Role>> AddRoleAsync(Role current)
        {
            return await ValidateIfNotAlreadyExistsAsync(current).ToAsync()
               .MapAsync(async ac =>
               {
                   ac.Id = NewId.NextGuid();
                   await ctx.Roles.AddAsync(ac);
                   ctx.SetAuditAndSpecialFields();
                   return ac;
               });
        }

        private async Task<Either<IServiceAndFeatureError, Role>> ValidateIfNotAlreadyExistsAsync(Role current)
        {
            var exists = await ctx.Roles.AnyAsync(a => a.Code == current.Code);
            return exists
                ? new ResourceAlreadyExistsError("Role", "Code", current.Code)
                : current;
        }

        private async Task<Either<IServiceAndFeatureError, Role>> ValidateIfNotAlreadyExistsWithOtherIdAsync(Role current)
        {
            var exists = await ctx.Roles.AnyAsync(a => a.Code == current.Code && a.Id != current.Id);

            return exists
                ? new ResourceAlreadyExistsError("Role", "Code", current.Code)
                : current;
        }
    }
}
