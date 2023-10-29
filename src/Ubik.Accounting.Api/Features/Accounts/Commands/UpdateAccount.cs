using MassTransit;
using MediatR;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.ApiService.DB.Enums;

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
            var msg = context.Message;

            //Check if the account is found
            var account = await _serviceManager.AccountService.GetAsync(msg.Id);
            if (account == null)
            {
                await context.RespondAsync(new AccountNotFoundException(msg.Id));
                return;
            }

            //Check if the account code already exists in other records
            bool exists = await _serviceManager.AccountService
                .IfExistsWithDifferentIdAsync(msg.Code, msg.Id);

            if (exists)
            {
                await context.RespondAsync(new AccountAlreadyExistsException(msg.Code));
                return;
            }

            //Check if the specified currency exists
            var curExists = await _serviceManager.AccountService.IfExistsCurrencyAsync(msg.CurrencyId);
            if(!curExists)
            {
                await context.RespondAsync(new AccountCurrencyNotFoundException(msg.CurrencyId));
                return;
            }

            //Modify the found account
            account = msg.ToAccount(account);

            ////Store and publish
            _serviceManager.AccountService.Update(account);
            await _publishEndpoint.Publish(account.ToAccountUpdated(), CancellationToken.None);
            await _serviceManager.SaveAsync();

            //Response
            await context.RespondAsync(account.ToUpdateAccountResult());
        }
    }
}
