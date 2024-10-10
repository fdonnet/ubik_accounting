using LanguageExt;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Users.Mappers;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Tenants.Commands;
using Ubik.Security.Contracts.Tenants.Events;
using Ubik.Security.Contracts.Users.Commands;
using Ubik.Security.Contracts.Users.Events;

namespace Ubik.Security.Api.Features.Users.Services
{
    public class UsersCommandsService(SecurityDbContext ctx, IUserAuthProviderService authUserProviderService, IPublishEndpoint publishEndpoint)
        : IUsersCommandsService
    {
        public async Task<Either<IServiceAndFeatureError, User>> AddAsync(AddUserCommand userCommand)
        {
            //TODO: Enhance this part... dirty asf to maintain aligned systems (DB + auth)
            //Ad the ID to be aligned with DB or use email as identifier ?

            //First step DB
            var result = await AddUserAsync(userCommand.ToUser());

            return await result.MatchAsync(
                RightAsync: async okDb =>
                {
                    var resultAuth = await authUserProviderService.AddUserAsync(userCommand);

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
            var result = await AddNewTenantAndAttachToTheUserAsync(userId, command.ToTenant());

            return await result.MatchAsync<Either<IServiceAndFeatureError, Tenant>>(
            RightAsync: async ok =>
            {
                var tenandAdded = new UserTenantAdded()
                {
                    UserId = userId,
                    NewLinkedTenantCreated = ok.ToTenantAdded()
                };
                await publishEndpoint.Publish(tenandAdded, CancellationToken.None);
                await ctx.SaveChangesAsync();
                return ok;
            },
            Left: err =>
            {
                return Prelude.Left(err);
            });
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> AddNewTenantAndAttachToTheUserAsync(Guid userId, Tenant tenant)
        {
            var result = await GetAsync(userId)
                .BindAsync(u => AddTenantWithUserEmailAsync(tenant, u.Email))
                .BindAsync(t => AddUserTenantLinkAsync(userId, t));

            return result;
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> AddTenantWithUserEmailAsync(Tenant tennant, string userEmail)
        {
            //TODO generate better tenant unique code or use a owner field (for the unique constrain)
            tennant.Code = GenerateTenantCode(tennant.Code, userEmail);
            return await AddTenantAsync(tennant);
        }

        private static string GenerateTenantCode(string tenantCode, string userEmail)
        {
            var userEmailForCode = userEmail.Split("@")[0];
            var code = tenantCode + " - " + userEmailForCode;
            if (code.Length >= 50)
                code = code[..50];
            return code;
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> AddUserTenantLinkAsync(Guid userId, Tenant tenant)
        {
            return await ValidateIfTenantLinkNotAlreadyExistsAsync(userId, tenant)
               .MapAsync(async t =>
               {
                   var ut = new UserTenant()
                   {
                       Id = NewId.NextGuid(),
                       TenantId = t.Id,
                       UserId = userId,
                   };

                   await ctx.UsersTenants.AddAsync(ut);
                   ctx.SetAuditAndSpecialFields();
                   return tenant;
               });
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> ValidateIfTenantLinkNotAlreadyExistsAsync(Guid userId, Tenant tenant)
        {
            var result = await ctx.UsersTenants.FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TenantId == tenant.Id);

            return result == null
                ? tenant
                : new ResourceAlreadyExistsError("UserTenant(link)", "UserId/TenantId", $"{userId}/{tenant.Id}");
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> AddTenantAsync(Tenant current)
        {
            return await ValidateIfTenantNotAlreadyExistsAsync(current)
               .MapAsync(async ac =>
               {
                   ac.Id = NewId.NextGuid();
                   await ctx.Tenants.AddAsync(ac);
                   ctx.SetAuditAndSpecialFields();
                   return ac;
               });
        }

        private async Task<Either<IServiceAndFeatureError, Tenant>> ValidateIfTenantNotAlreadyExistsAsync(Tenant current)
        {
            var exists = await ctx.Tenants.AnyAsync(a => a.Code == current.Code);
            return exists
                ? new ResourceAlreadyExistsError("Tenant", "Code", current.Code)
                : current;
        }

        private async Task<Either<IServiceAndFeatureError, User>> AddUserAsync(User user)
        {
            return await ValidateIfNotAlreadyExistsAsync(user)
                .MapAsync(async ac =>
                {
                    ac.Id = NewId.NextGuid();
                    await ctx.Users.AddAsync(ac);
                    ctx.SetAuditAndSpecialFields();
                    return ac;
                });
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
