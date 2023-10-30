using MassTransit;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Ubik.Accounting.Contracts.Accounts.Results;
using Ubik.ApiService.Common.Exceptions;
using Ubik.ApiService.DB.Enums;

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

            if(result.IsSuccess)
            {
                //Store and publish AccountAdded event
                await _publishEndpoint.Publish(result.Result.ToAccountAdded(), CancellationToken.None);
                await _serviceManager.SaveAsync();
                await context.RespondAsync(result.Result.ToAddAccountResult());
            }
            else
                await context.RespondAsync(result.Exception);
        }
    }
}
