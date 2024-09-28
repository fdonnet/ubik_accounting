using MassTransit;
using Ubik.Security.Api.Features.Users.Mappers;
using Ubik.Security.Api.Features.Users.Services;
using Ubik.Security.Contracts.Users.Commands;

namespace Ubik.Security.Api.Features.Users.Commands
{
    public class AddUserConsumer(IServiceManager serviceManager, IUserAuthProviderService userAuthProviderService, IPublishEndpoint publishEndpoint)
        : IConsumer<AddUserCommand>
    {
        private readonly IServiceManager _serviceManager = serviceManager;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        private readonly IUserAuthProviderService _userAuthProviderService = userAuthProviderService;

        public async Task Consume(ConsumeContext<AddUserCommand> context)
        {
            var msg = context.Message;
            //TODO: Enhance this part... dirty asf to maintain aligned systems (DB + auth)
            //Ad the ID to be aligned with DB or use email as identifier ?

            //First step DB
            var result = await _serviceManager.UserManagementService.AddAsync(msg.ToUser());

            await result.Match(
                Right: async okDb =>
                {
                    var resultAuth = await _userAuthProviderService.AddUserAsync(msg);
                    await result.Match(
                        Right: async okAfterAuth =>
                        {
                            //Store and publish UserAdded event (auth + DB = OK)
                            await _publishEndpoint.Publish(okAfterAuth.ToUserAdded(), CancellationToken.None);
                            await _serviceManager.SaveAsync();
                            await context.RespondAsync(okAfterAuth.ToAddUserResult());
                        },
                        Left: async errAuth =>
                        {
                            //Remove user from DB if auth add failed
                            var delDbResult = await _serviceManager.UserManagementService.ExecuteDeleteAsync(okDb.Id);

                            await delDbResult.Match(
                                Right: async okDelInDb => await context.RespondAsync(errAuth),
                                Left: async koDelInDb => await context.RespondAsync(koDelInDb));
                        });
                },
                //DB not ok
                Left: async errDB => await context.RespondAsync(errDB));
        }
    }
}
