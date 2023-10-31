using MassTransit;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;

namespace Ubik.Accounting.Api.Features.AccountGroups.Commands
{
    public class UpdateAccountGroupConsumer : IConsumer<UpdateAccountGroupCommand>
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;

        public UpdateAccountGroupConsumer(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
        {
            _serviceManager = serviceManager;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<UpdateAccountGroupCommand> context)
        {
            var accountGrp = context.Message.ToAccountGroup();
            var res = await _serviceManager.AccountGroupService.UpdateAsync(accountGrp);

            if (res.IsSuccess)
            {
                try
                {
                    await _publishEndpoint.Publish(accountGrp.ToAccountGroupUpdated(), CancellationToken.None);
                    await _serviceManager.SaveAsync();

                    await context.RespondAsync(res.Result.ToUpdateAccountGroupResult());
                }
                catch (UpdateDbConcurrencyException)
                {
                    await context.RespondAsync(new AccountGroupUpdateConcurrencyExeception(context.Message.Version));
                }
            }
            else
                await context.RespondAsync(res.Exception);
        }
    }
}
