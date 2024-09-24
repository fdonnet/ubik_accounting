using MassTransit;
using Ubik.Security.Api.Features.Users.Mappers;
using Ubik.Security.Contracts.Users.Commands;

namespace Ubik.Security.Api.Features.Users.Commands
{
    public class AddUserConsumer(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
        : IConsumer<AddUserCommand>
    {
        private readonly IServiceManager _serviceManager = serviceManager;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

        public async Task Consume(ConsumeContext<AddUserCommand> context)
        {
            var msg = context.Message.ToUser();

            var result = await _serviceManager.UserManagementService.AddAsync(msg);

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
