using MassTransit;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Contracts.AccountGroups.Events;
using Ubik.Accounting.Contracts.AccountGroups.Results;

namespace Ubik.Accounting.Api.Features.AccountGroups.Commands
{
    public class DeleteAccountGroupConsumer : IConsumer<DeleteAccountGroupCommand>
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;

        public DeleteAccountGroupConsumer(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
        {
            _serviceManager = serviceManager;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<DeleteAccountGroupCommand> context)
        {
            var res = await _serviceManager.AccountGroupService.ExecuteDeleteAsync(context.Message.Id);

            if (res.IsSuccess)
            {
                await _publishEndpoint.Publish(new AccountGroupDeleted { Id = context.Message.Id }, CancellationToken.None);
                await _serviceManager.SaveAsync();
                await context.RespondAsync<DeleteAccountGroupResult>(new { Deleted = true });
            }
            else
                await context.RespondAsync(res.Exception);
        }
    }
}
