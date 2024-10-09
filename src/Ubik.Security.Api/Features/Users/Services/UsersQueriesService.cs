using LanguageExt;
using LanguageExt.Pipes;
using LanguageExt.Pretty;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Users.Errors;
using Ubik.Security.Api.Models;

namespace Ubik.Security.Api.Features.Users.Services
{
    public class UsersQueriesService(SecurityDbContext ctx) : IUsersQueriesService
    {
        public async Task<Either<IServiceAndFeatureError, User>> GetAsync(Guid id)
        {
            var result = await ctx.Users.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("User", "Id", id.ToString())
                : result;
        }

        public async Task<Either<IServiceAndFeatureError, User>> GetAsync(string email)
        {
            var result = await ctx.Users.FirstOrDefaultAsync(u => u.Email == email);

            return result == null
                ? new ResourceNotFoundError("User", "Email", email.ToString())
                : result;
        }

        public async Task<Either<IServiceAndFeatureError, Tenant>> GetUserSelectedTenantAsync(Guid userId)
        {
            var result =  await GetAsync(userId).ToAsync()
                .Bind(u => GetTenant(u.SelectedTenantId).ToAsync());

            return result;
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> GetTenant(Guid? tenantId)
        {
            if (tenantId == null) return new UserTenantNotFound(tenantId);

            var result = await ctx.Tenants.FindAsync(tenantId);

            if (result == null)
                return new UserTenantNotFound(tenantId);

            return result;
        }
    }
}

