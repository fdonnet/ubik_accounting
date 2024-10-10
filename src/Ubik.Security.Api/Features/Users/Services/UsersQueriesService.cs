using Dapper;
using LanguageExt;
using LanguageExt.Pipes;
using LanguageExt.Pretty;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;
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
            var result = await GetAsync(userId)
                .BindAsync(u => GetTenantAsync(u.SelectedTenantId))
                .BindAsync(t => ValidateIfExistsForTheUserAsync(t, userId));

            return result;
        }

        public async Task<Either<IServiceAndFeatureError, Tenant>> GetUserTenantAsync(Guid userId, Guid tenantId)
        {
            var result = await GetAsync(userId)
                 .BindAsync(u => GetTenantAsync(tenantId))
                 .BindAsync(t => ValidateIfExistsForTheUserAsync(t, userId));

            return result;
        }

        public async Task<Either<IServiceAndFeatureError, IEnumerable<Tenant>>> GetUserAllTenantsAsync(Guid userId)
        {
            var tenants = (await GetAsync(userId))
                .MapAsync(async u =>
                {
                    var p = new DynamicParameters();
                    p.Add("@userid", u.Id);

                    var con = ctx.Database.GetDbConnection();
                    var sql =
                        """
                        SELECT t.*
                        FROM tenants t
                        INNER JOIN users_tenants ut ON ut.user_id = @userid
                        """;

                    return await con.QueryAsync<Tenant>(sql, p);
                });

            return await tenants;
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> GetTenantAsync(Guid? tenantId)
        {
            if (tenantId == null) return new UserTenantNotFound(tenantId);

            var result = await ctx.Tenants.FindAsync(tenantId);

            return result == null
                ? new UserTenantNotFound(tenantId)
                : result;
        }


        private async Task<Either<IServiceAndFeatureError, Tenant>> ValidateIfExistsForTheUserAsync(Tenant tenant, Guid userId)
        {
            var result = await ctx.UsersTenants.FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TenantId == tenant.Id);

            return result == null
                ? new UserTenantNotFound(tenant.Id)
                : tenant;
        }
    }
}

