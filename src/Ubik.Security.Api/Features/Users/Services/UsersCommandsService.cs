using LanguageExt;
using LanguageExt.Pipes;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ubik.ApiService.Common.Errors;
using Ubik.ApiService.Common.Services;
using Ubik.Security.Api.Data;
using Ubik.Security.Api.Features.Users.Mappers;
using Ubik.Security.Api.Models;
using Ubik.Security.Contracts.Users.Commands;
using Ubik.Security.Contracts.Users.Results;

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

        private async Task<Either<IServiceAndFeatureError, User>> AddUserAsync(User user)
        {
            return await ValidateIfNotAlreadyExistsAsync(user).ToAsync()
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
            return await GetAsync(id).ToAsync()
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
