using Dapper;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Users.Errors;
using Ubik.Security.Api.Mappers;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Authorizations.Results;
using Ubik.Security.Contracts.Users.Results;

namespace Ubik.Security.Api.Features.Users.Services
{
    public class UsersQueriesService(SecurityDbContext ctx, ICurrentUser currentUser) : IUsersQueriesService
    {

        public async Task<Either<IFeatureError, User>> GetUserInSelectedTenantAsync(Guid id)
        {
            var p = new DynamicParameters();
            p.Add("@user_id", id);
            p.Add("@tenant_id", currentUser.TenantId);

            var con = ctx.Database.GetDbConnection();
            var sql =
                """
                SELECT u.*
                FROM users u
                INNER JOIN users_tenants ut ON ut.user_id = u.id
                WHERE u.id = @user_id
                AND ut.tenant_id = @tenant_id
                """;

            var result = await con.QueryFirstOrDefaultAsync<User>(sql, p);

            return result == null
                ? new ResourceNotFoundError("User", "Id", id.ToString())
                : result;
        }

        public async Task<Either<IFeatureError, User>> GetAsync(Guid id)
        {
            var result = await ctx.Users.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("User", "Id", id.ToString())
                : result;
        }

        public async Task<Either<IFeatureError, User>> GetAsync(string email)
        {
            var result = await ctx.Users.FirstOrDefaultAsync(u => u.Email == email);

            return result == null
                ? new ResourceNotFoundError("User", "Email", email.ToString())
                : result;
        }

        public async Task<Either<IFeatureError, UserAdminOrMeResult>> GetUserWithAuhtorizationsByTenants(string email)
        {
            return await GetAsync(email)
                    .MapAsync(async u =>
                    {
                        var authorizations = await GetAllAuthorizationByTenantAsyc(u.Id);
                        var result = u.ToUserAdminOrMeResult(authorizations);
                        return result;
                    });
        }

        public async Task<Either<IFeatureError, UserAdminOrMeResult>> GetUserWithAuhtorizationsByTenants(Guid id)
        {
            return await GetAsync(id)
                    .MapAsync(async u =>
                    {
                        var authorizations = await GetAllAuthorizationByTenantAsyc(u.Id);
                        var result = u.ToUserAdminOrMeResult(authorizations);
                        return result;
                    });
        }

        public async Task<Either<IFeatureError, IEnumerable<Role>>> GetUserRolesInSelectedTenantAsync(Guid id)
        {
            return await GetUserInSelectedTenantAsync(id)
                .MapAsync(async u =>
                {
                    var p = new DynamicParameters();
                    p.Add("@user_id", u.Id);
                    p.Add("@tenant_id", currentUser.TenantId);

                    var con = ctx.Database.GetDbConnection();
                    var sql =
                        """
                        SELECT r.*
                        FROM roles r
                        INNER JOIN users_roles_by_tenant urt ON urt.role_id = r.id
                        INNER JOIN users_tenants ut ON ut.id = urt.user_tenant_id
                        WHERE ut.user_id = @user_id
                        AND ut.tenant_id = @tenant_id
                        """;

                    return await con.QueryAsync<Role>(sql, p);
                });
        }

        public async Task<Either<IFeatureError, Role>> GetUserRoleInSelectedTenantAsync(Guid id, Guid roleId)
        {
            return await GetUserInSelectedTenantAsync(id)
                .BindAsync(u => GetRoleForUserAsync(u.Id, roleId));
        }

        private async Task<Either<IFeatureError, Role>> GetRoleForUserAsync(Guid id,Guid roleId)
        {
            var p = new DynamicParameters();
            p.Add("@user_id", id);
            p.Add("@tenant_id", currentUser.TenantId);
            p.Add("@role_id", roleId);

            var con = ctx.Database.GetDbConnection();
            var sql =
                """
                SELECT r.*
                FROM roles r
                INNER JOIN users_roles_by_tenant urt ON urt.role_id = r.id
                INNER JOIN users_tenants ut ON ut.id = urt.user_tenant_id
                WHERE ut.user_id = @user_id
                AND ut.tenant_id = @tenant_id
                AND r.id = @role_id
                """;

            var result = await con.QuerySingleOrDefaultAsync<Role>(sql, p);

            if (result == null)
                return new ResourceNotFoundError("Role", "Id", roleId.ToString());
            else
                return result;
        }

        private async Task<Dictionary<Guid, List<AuthorizationStandardResult>>> GetAllAuthorizationByTenantAsyc(Guid userId)
        {
            var p = new DynamicParameters();
            p.Add("@userid", userId);

            var con = ctx.Database.GetDbConnection();
            var sql =
                """
                SELECT DISTINCT ut.tenant_id, a.*
                FROM users u
                INNER JOIN users_tenants ut ON ut.user_id = u.id
                INNER JOIN users_roles_by_tenant urt ON urt.user_tenant_id = ut.id
                INNER JOIN roles_authorizations ra ON ra.role_id = urt.role_id
                INNER JOIN authorizations a ON a.id = ra.authorization_id
                WHERE u.id = @userid
                """;

            var result = await con.QueryAsync<dynamic>(sql, p);

            var dic = new Dictionary<Guid, List<AuthorizationStandardResult>>();

            foreach (var record in result)
            {
                if (!dic.ContainsKey(record.tenant_id))
                {
#pragma warning disable IDE0028 // Simplify collection initialization (not working good with Dynamic)
                    dic.Add(record.tenant_id, new List<AuthorizationStandardResult>());
#pragma warning restore IDE0028 // Simplify collection initialization (not working good with Dynamic)
                }
                dic[record.tenant_id].Add(new AuthorizationStandardResult()
                {
                    Code = record.code,
                    Description = record.description,
                    Id = record.id,
                    Label = record.label,
                    Version = record.version
                });
            }

            return dic;
        }

        public async Task<Either<IFeatureError, Tenant>> GetUserSelectedTenantAsync(Guid userId)
        {
            var result = await GetAsync(userId)
                .BindAsync(u => GetTenantAsync(u.SelectedTenantId))
                .BindAsync(t => ValidateIfExistsForTheUserAsync(t, userId));

            return result;
        }

        public async Task<Either<IFeatureError, Tenant>> GetUserTenantAsync(Guid userId, Guid tenantId)
        {
            var result = await GetAsync(userId)
                 .BindAsync(u => GetTenantAsync(tenantId))
                 .BindAsync(t => ValidateIfExistsForTheUserAsync(t, userId));

            return result;
        }

        public async Task<Either<IFeatureError, IEnumerable<Tenant>>> GetUserAllTenantsAsync(Guid userId)
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
                        INNER JOIN users_tenants ut ON ut.tenant_id = t.id
                        WHERE ut.user_id = @userid
                        """;

                    return await con.QueryAsync<Tenant>(sql, p);
                });

            return await tenants;
        }

        private async Task<Either<IFeatureError, Tenant>> GetTenantAsync(Guid? tenantId)
        {
            if (tenantId == null) return new UserTenantNotFound(tenantId);

            var result = await ctx.Tenants.FindAsync(tenantId);

            return result == null
                ? new UserTenantNotFound(tenantId)
                : result;
        }


        private async Task<Either<IFeatureError, Tenant>> ValidateIfExistsForTheUserAsync(Tenant tenant, Guid userId)
        {
            var result = await ctx.UsersTenants.FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TenantId == tenant.Id);

            return result == null
                ? new UserTenantNotFound(tenant.Id)
                : tenant;
        }
    }
}

