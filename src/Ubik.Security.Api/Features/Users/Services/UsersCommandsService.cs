using Dapper;
using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Users.Errors;
using Ubik.Security.Api.Mappers;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Tenants.Commands;
using Ubik.Security.Contracts.Users.Commands;
using Ubik.Security.Contracts.Users.Events;


//Write tests for this part !!!
namespace Ubik.Security.Api.Features.Users.Services
{
    public class UsersCommandsService(SecurityDbContext ctx
        , IUserAuthProviderService authUserProviderService
        , IPublishEndpoint publishEndpoint
        , ICurrentUser currentUser)
        : IUsersCommandsService
    {
        public async Task<Either<IServiceAndFeatureError, User>> AddAsync(AddUserCommand command)
        {
            //TODO: Enhance this part... dirty asf

            //First step DB
            var result = await ValidateIfNotAlreadyExistsAsync(command.ToUser())
                            .BindAsync(AddInDbContextAsync);

            return await result.MatchAsync(
                RightAsync: async okDb =>
                {
                    var resultAuth = await authUserProviderService.AddUserAsync(command);

                    return await resultAuth.MatchAsync<Either<IServiceAndFeatureError, User>>(
                        RightAsync: async okAfterAuth =>
                        {
                            //Store and publish UserAdded event (auth + DB = OK)
                            await publishEndpoint.Publish(okDb.ToUserAdded(), CancellationToken.None);
                            await ctx.SaveChangesAsync();
                            return okDb;
                        },
                        LeftAsync: async errAuth =>
                        {
                            //Remove user from DB if auth add failed
                            var delDbResult = await ExecuteDeleteAsync(okDb.Id);

                            return delDbResult.Match(
                                Right: okDb =>
                                {
                                    return Prelude.Left(errAuth);
                                },
                                Left: noDelInDb =>
                                {
                                    return Prelude.Left(noDelInDb);
                                });
                        });
                },
               //DB not ok
               Left: err =>
               {
                   return Prelude.Left(err);
               });
        }

        public async Task<Either<IServiceAndFeatureError, Tenant>> AddNewTenantAsync(Guid userId, AddTenantCommand command)
        {
            var model = command.ToTenant();

            var tenantAdded = await GetAsync(userId)
                .BindAsync(u => CompleteTenantCode(model, u.Email))
                .BindAsync(ValidateIfTenantNotAlreadyExistsAsync)
                .BindAsync(AddTenantInDbContextAsync);

            return await tenantAdded
                .BindAsync(t => ValidateIfTenantLinkNotAlreadyExistsAsync(userId, t))
                .BindAsync(t => AddUserTenantLinkInDbContextAsync(userId, t))
                .BindAsync(GetTenantUserManagementRole)
                .BindAsync(r => AddTenantUserManagerRoleToTheUserInDbContextAsync(r.Item1, r.Item2.Id))
                .BindAsync(t => tenantAdded)
                .BindAsync(t => AddSaveAndPublishAsync(t, userId));
        }

        public async Task<Either<IServiceAndFeatureError, Role>> AddRoleInTenantAsync(Guid userId, Guid roleId)
        {
            return await GetUserInSelectedTenantAsync(userId)
                .BindAsync(u => CheckIfRoleExistInTenantOrBaseRole(roleId))
                .BindAsync(r => GetUserTenantLinkForRole(userId, r))
                .BindAsync(utr => CheckIfUserTenantRoleAlreadyExists(utr.Item1, utr.Item2))
                .BindAsync(utr => AddRoleToUserInTenantInDbContextAsync(utr.Item1.Id, utr.Item2))
                .BindAsync(r => AddRoleInTenantSaveAndPublishAsync(r, userId));
        }

        private async Task<Either<IServiceAndFeatureError, Role>> AddRoleInTenantSaveAndPublishAsync(Role current, Guid userId)
        {
            var userRoleAddedToTenant = new UserRoleAddedToTenant()
            {
                UserId = userId,
                RoleId = current.Id,
                TenantId = (Guid)currentUser.TenantId!
            };

            await publishEndpoint.Publish(userRoleAddedToTenant, CancellationToken.None);
            await ctx.SaveChangesAsync();
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> AddSaveAndPublishAsync(Tenant current, Guid userId)
        {
            var tenandAdded = new UserTenantAdded()
            {
                UserId = userId,
                NewLinkedTenantCreated = current.ToTenantAdded()
            };
            await publishEndpoint.Publish(tenandAdded, CancellationToken.None);
            await ctx.SaveChangesAsync();
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, (UserTenant, Role)>> GetUserTenantLinkForRole(Guid userId, Role role)
        {
            var result = await ctx.UsersTenants.SingleOrDefaultAsync(ut => ut.UserId == userId
                                                            && ut.TenantId == currentUser.TenantId);

            return result == null
                ? new ResourceNotFoundError("UserTenant", "UserId/TenantId", $"{userId}/{currentUser.TenantId}")
                : (result, role);
        }

        private async Task<Either<IServiceAndFeatureError, (UserTenant, Role)>> CheckIfUserTenantRoleAlreadyExists(UserTenant userTenant, Role role)
        {
            var result = await ctx.UserRolesByTenants.SingleOrDefaultAsync(ut => ut.UserTenantId == userTenant.Id
                                                            && ut.RoleId == role.Id);

            return result == null
                ? (userTenant, role)
                : new ResourceAlreadyExistsError("UserRoleByTenant", "UserTenantId/RoleId", $"{userTenant.Id}/{role.Id}");
        }

        private async Task<Either<IServiceAndFeatureError, Role>> AddRoleToUserInTenantInDbContextAsync(Guid userTenantId, Role role)
        {
            var roleInTenantByUser = new UserRoleByTenant()
            {
                RoleId = role.Id,
                UserTenantId = userTenantId,
            };

            await ctx.UserRolesByTenants.AddAsync(roleInTenantByUser);
            ctx.SetAuditAndSpecialFields();
            return role;
        }

        private async Task<Either<IServiceAndFeatureError, Role>>
                CheckIfRoleExistInTenantOrBaseRole(Guid roleId)
        {
            var p = new DynamicParameters();
            p.Add("@tenant_id", currentUser.TenantId);
            p.Add("@role_id", roleId);

            var con = ctx.Database.GetDbConnection();
            var sql =
                """
                SELECT r.*
                FROM roles r
                WHERE (r.tenant_id = @tenant_id OR r.tenant_id IS NULL)
                AND r.id = @role_id
                """;

            var result = await con.QuerySingleOrDefaultAsync<Role>(sql, p);

            return result == null
                ? (Either<IServiceAndFeatureError, Role>)new ResourceNotFoundError("Role", "Id", roleId.ToString())
                : (Either<IServiceAndFeatureError, Role>)result;
        }

        private async Task<Either<IServiceAndFeatureError, User>> GetUserInSelectedTenantAsync(Guid id)
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

        private static async Task<Either<IServiceAndFeatureError,Tenant>> CompleteTenantCode(Tenant current, string userEmail)
        {
            var userEmailForCode = userEmail.Split("@")[0];
            current.Code = current.Code + " - " + userEmailForCode;

            if (current.Code.Length >= 50)
                current.Code = current.Code[..50];

            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, UserTenant>> AddUserTenantLinkInDbContextAsync(Guid userId, Tenant current)
        {
            var ut = new UserTenant()
            {
                Id = NewId.NextGuid(),
                TenantId = current.Id,
                UserId = userId,
            };

            await ctx.UsersTenants.AddAsync(ut);
            ctx.SetAuditAndSpecialFields();
            return ut;
        }

        private async Task<Either<IServiceAndFeatureError, UserRoleByTenant>> AddTenantUserManagerRoleToTheUserInDbContextAsync(Role current, Guid userTenantLinkId)
        {
            var newRoleForUser = new UserRoleByTenant()
            {
                Id = NewId.NextGuid(),
                RoleId = current.Id,
                UserTenantId = userTenantLinkId,
            };

            await ctx.UserRolesByTenants.AddAsync(newRoleForUser);
            ctx.SetAuditAndSpecialFields();
            return newRoleForUser;
        }

        private async Task<Either<IServiceAndFeatureError, (Role, UserTenant)>> GetTenantUserManagementRole(UserTenant current)
        {
            var result = await ctx.Roles.FirstOrDefaultAsync(r => r.Code == "usrmgt_all_rw");

            return result == null
                ? new UserCannotGetMainUsrMgtRole()
                : (result, current);
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> ValidateIfTenantLinkNotAlreadyExistsAsync(Guid userId, Tenant tenant)
        {
            var result = await ctx.UsersTenants.FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TenantId == tenant.Id);

            return result == null
                ? tenant
                : new ResourceAlreadyExistsError("UserTenant(link)", "UserId/TenantId", $"{userId}/{tenant.Id}");
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> AddTenantInDbContextAsync(Tenant current)
        {
            current.Id = NewId.NextGuid();
            await ctx.Tenants.AddAsync(current);
            ctx.SetAuditAndSpecialFields();
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> ValidateIfTenantNotAlreadyExistsAsync(Tenant current)
        {
            var exists = await ctx.Tenants.AnyAsync(a => a.Code == current.Code);
            return exists
                ? new ResourceAlreadyExistsError("Tenant", "Code", current.Code)
                : current;
        }

        private async Task<Either<IServiceAndFeatureError, User>> AddInDbContextAsync(User current)
        {
            current.Id = NewId.NextGuid();
            await ctx.Users.AddAsync(current);
            ctx.SetAuditAndSpecialFields();
            return current;
        }

        private async Task<Either<IServiceAndFeatureError, bool>> ExecuteDeleteAsync(Guid id)
        {
            return await GetAsync(id)
                .MapAsync(async ac =>
                {
                    await ctx.Users.Where(x => x.Id == id).ExecuteDeleteAsync();
                    return true;
                });
        }

        private async Task<Either<IServiceAndFeatureError, User>> GetAsync(Guid id)
        {
            var result = await ctx.Users.FindAsync(id);

            return result == null
                ? new ResourceNotFoundError("User", "Id", id.ToString())
                : result;
        }

        private async Task<Either<IServiceAndFeatureError, User>> ValidateIfNotAlreadyExistsAsync(User user)
        {
            var exists = await ctx.Users.AnyAsync(a => a.Email == user.Email);
            return exists
                ? new ResourceAlreadyExistsError("User", "Email", user.Email)
                : user;
        }


    }
}
