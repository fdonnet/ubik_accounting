﻿using MassTransit;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
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
            var res = await _serviceManager.AccountGroupService.DeleteAsync(context.Message.Id);

            await res.Match(
                Right: async r =>
                {
                    await _publishEndpoint.Publish(new AccountGroupsDeleted { AccountGroups = r.ToAccountGroupDeleted() }, CancellationToken.None);
                    await _serviceManager.SaveAsync();
                    await context.RespondAsync(new DeleteAccountGroupResults { AccountGroups = r.ToDeleteAccountGroupsResult() });
                },
                Left: async err => await context.RespondAsync(err));
        }
    }
}
