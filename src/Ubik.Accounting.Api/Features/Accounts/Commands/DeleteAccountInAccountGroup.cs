using MassTransit;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Contracts.Accounts.Commands;

namespace Ubik.Accounting.Api.Features.Accounts.Commands
{
    public class DeleteAccountInAccountGroupConsumer : IConsumer<DeleteAccountInAccountGroupCommand>
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;

        public DeleteAccountInAccountGroupConsumer(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
        {
            _serviceManager = serviceManager;
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<DeleteAccountInAccountGroupCommand> context)
        {
            var msg = context.Message;

            var result = await _serviceManager.AccountService.DeleteFromAccountGroupAsync(msg.AccountId, msg.AccountGroupId);

            await result.Match(
                Right: async r =>
                {
                    //Store and publish AccountAdded event
                    await _publishEndpoint.Publish(r.ToAccountDeletedInAccountGroup(), CancellationToken.None);
                    await _serviceManager.SaveAsync();
                    await context.RespondAsync(r.ToDeleteAccountInAccountGroupResult());
                },
                Left: async err => await context.RespondAsync(err));
        }
    }
}
