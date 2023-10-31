using MassTransit;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
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

            if (result.IsSuccess)
            {
                //Store and publish AccountGroupAdded event
                await _publishEndpoint.Publish(result.Result.ToAccountGroupAdded(), CancellationToken.None);
                await _serviceManager.SaveAsync();
                await context.RespondAsync(result.Result.ToAddAccountGroupResult());
            }
            else
                await context.RespondAsync(result.Exception);
        }
    }
}
