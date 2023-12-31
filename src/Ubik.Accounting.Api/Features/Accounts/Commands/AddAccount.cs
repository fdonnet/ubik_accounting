﻿using MassTransit;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Contracts.Accounts.Commands;

namespace Ubik.Accounting.Api.Features.Accounts.Commands
{
    public class AddAccountConsumer : IConsumer<AddAccountCommand>
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;

        public AddAccountConsumer(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
        {
            _serviceManager = serviceManager;
            _publishEndpoint = publishEndpoint;
        }
        public async Task Consume(ConsumeContext<AddAccountCommand> context)
        {
            var account = context.Message.ToAccount();

            var result = await _serviceManager.AccountService.AddAsync(account);

            await result.Match(
                Right: async r =>
                {
                    //Store and publish AccountAdded event
                    await _publishEndpoint.Publish(r.ToAccountAdded(), CancellationToken.None);
                    await _serviceManager.SaveAsync();
                    await context.RespondAsync(r.ToAddAccountResult());
                },
                Left: async err => await context.RespondAsync(err));
        }
    }
}
