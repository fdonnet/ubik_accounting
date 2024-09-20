using MassTransit;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Commands;

namespace Ubik.Accounting.Api.Features.Accounts.Commands
{
    public class AddAccountInAccountGroupConsumer : IConsumer<AddAccountInAccountGroupCommand>
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;

        public AddAccountInAccountGroupConsumer(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
        {
            _serviceManager = serviceManager;
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<AddAccountInAccountGroupCommand> context)
        {
            var msg = context.Message.ToAccountAccountGroup();

            var result = await _serviceManager.AccountService.AddInAccountGroupAsync(msg);

            await result.Match(
                Right: async r =>
                {
                    //Store and publish AccountAdded event
                    await _publishEndpoint.Publish(r.ToAccountAddedInAccountGroup(), CancellationToken.None);
                    await _serviceManager.SaveAsync();
                    await context.RespondAsync(r.ToAddAccountInAccountGroupResult());
                },
                Left: async err => await context.RespondAsync(err));
        }
    }
}
