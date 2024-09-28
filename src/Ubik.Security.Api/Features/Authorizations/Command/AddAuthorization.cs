using MassTransit;
using Ubik.Security.Api.Features.Authorizations.Mappers;
using Ubik.Security.Contracts.Authorizations.Commands;

namespace Ubik.Security.Api.Features.Authorizations.Command
{
    public class AddAuthorizationConsumer(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
        : IConsumer<AddAuthorizationCommand>
    {
        private readonly IServiceManager _serviceManager = serviceManager;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

        public async Task Consume(ConsumeContext<AddAuthorizationCommand> context)
        {
            var command = context.Message.ToAuthorization();

            var result = await _serviceManager.AuthorizationService.AddAsync(command);

            await result.Match(
                Right: async r =>
                {
                    //Store and publish AccountGroupAdded event
                    await _publishEndpoint.Publish(r.ToAuthorizationAdded(), CancellationToken.None);
                    await _serviceManager.SaveAsync();
                    await context.RespondAsync(r.ToAddAuthorizationResult());
                },
                Left: async err => await context.RespondAsync(err));
        }

    }
}
