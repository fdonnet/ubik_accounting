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

            var accountExists = await _serviceManager.AccountService.IfExistsAsync(account.Code);
            if (accountExists)
            {
                await context.RespondAsync(new AccountAlreadyExistsException(account.Code));
                return;
            }
            //Check if the specified currency exists
            var curExists = await _serviceManager.AccountService.IfExistsCurrencyAsync(account.CurrencyId);
            if (!curExists)
                throw new AccountCurrencyNotFoundException(account.CurrencyId);

            //Store and publish AccountAdded evenet
            await _serviceManager.AccountService.AddAsync(account);
            await _publishEndpoint.Publish(account.ToAccountAdded(), CancellationToken.None);
            await _serviceManager.SaveAsync();

            //Response
            await context.RespondAsync(account.ToAddAccountResult());
        }
    }
}
