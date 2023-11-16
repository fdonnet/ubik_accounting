using MassTransit;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Api.Features.AccountGroups.Errors;

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

            await res.Match(
                Right: async r =>
                {
                    try
                    {
                        await _publishEndpoint.Publish(accountGrp.ToAccountGroupUpdated(), CancellationToken.None);
                        await _serviceManager.SaveAsync();

                        await context.RespondAsync(r.ToUpdateAccountGroupResult());
                    }
                    catch (UpdateDbConcurrencyException)
                    {
                        await context.RespondAsync(new AccountGroupUpdateConcurrencyError(context.Message.Version));
                    }
                },
                Left: async err => await context.RespondAsync(err));
        }
    }
}
