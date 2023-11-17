using MassTransit;
using Ubik.Accounting.Api.Features.Accounts.Errors;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Accounts.Commands
{
    public class UpdateAccountConsumer : IConsumer<UpdateAccountCommand>
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;

        public UpdateAccountConsumer(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
        {
            _serviceManager = serviceManager;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<UpdateAccountCommand> context)
        {
            var account = context.Message.ToAccount();
            var res = await _serviceManager.AccountService.UpdateAsync(account);

            await res.Match(
                Right: async r =>
                {
                    try
                    {
                        await _publishEndpoint.Publish(account.ToAccountUpdated(), CancellationToken.None);
                        await _serviceManager.SaveAsync();

                        await context.RespondAsync(r.ToUpdateAccountResult());
                    }
                    catch (UpdateDbConcurrencyException)
                    {
                        await context.RespondAsync(new AccountUpdateConcurrencyError(context.Message.Version));
                    }
                },
                Left: async err => await context.RespondAsync(err));
        }
    }
}
