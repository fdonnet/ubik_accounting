using MassTransit;
using Ubik.ApiService.Common.Exceptions;
using Ubik.Accounting.Contracts.AccountGroups.Commands;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.AccountGroups.Commands
{
    public class UpdateAccountGroupConsumer(IServiceManager serviceManager, IPublishEndpoint publishEndpoint) : IConsumer<UpdateAccountGroupCommand>
    {
        private readonly IServiceManager _serviceManager = serviceManager;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

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
                        await context.RespondAsync(new ResourceUpdateConcurrencyError("AccountGroup",context.Message.Version.ToString()));
                    }
                },
                Left: async err => await context.RespondAsync(err));
        }
    }
}
