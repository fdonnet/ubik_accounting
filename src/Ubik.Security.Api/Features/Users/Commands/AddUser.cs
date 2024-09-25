using LanguageExt;
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
            //TODO: delete user from auth if it's not a success in the DB (change the way)
            // For the moment, the DB is protected but the Auth provider can have orphan users and
            // it's not good at all. (or do the inverse)

            //TODO: put check in place (email validation etc)
            var result = await _userAuthProviderService.AddUserAsync(msg).ToAsync()
                .Bind(ok => _serviceManager.UserManagementService.AddAsync(msg.ToUser()).ToAsync());

            await result.Match(
                Right: async r =>
                {
                    //Store and publish AccountAdded event
                    await _publishEndpoint.Publish(r.ToUserAdded(), CancellationToken.None);
                    await _serviceManager.SaveAsync();
                    await context.RespondAsync(r.ToAddUserResult());
                },
                Left: async err => await context.RespondAsync(err));
        }
    }
}
