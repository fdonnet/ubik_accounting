using MassTransit;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Contracts.AccountGroups.Commands;

namespace Ubik.Accounting.Api.Features.AccountGroups.Commands
{

    public class AddAccountGroupConsumer : IConsumer<AddAccountGroupCommand>
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;

        public AddAccountGroupConsumer(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
        {
            _serviceManager = serviceManager;
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<AddAccountGroupCommand> context)
        {
            var account = context.Message.ToAccountGroup();

            var result = await _serviceManager.AccountGroupService.AddAsync(account);

            await result.Match(
                Right: async r =>
                {
                    //Store and publish AccountGroupAdded event
                    await _publishEndpoint.Publish(r.ToAccountGroupAdded(), CancellationToken.None);
                    await _serviceManager.SaveAsync();
                    await context.RespondAsync(r.ToAddAccountGroupResult());
                },
                Left: async err => await context.RespondAsync(err));
        }
    }
}
