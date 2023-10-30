﻿using MassTransit;
using MassTransit.Transports;
using MediatR;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;
using System.Threading;
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
            var account = context.Message.ToAccount();
            var res = await _serviceManager.AccountService.UpdateAsync(account);

            if(res.IsSuccess)
            {
                try
                {
                    await _publishEndpoint.Publish(account.ToAccountUpdated(), CancellationToken.None);
                    await _serviceManager.SaveAsync();

                    await context.RespondAsync(res.Result.ToUpdateAccountResult());
                }
                catch (DBConcurrencyException)
                {
                    await context.RespondAsync(new AccountUpdateConcurrencyExeception(context.Message.Version));
                }
            }
            else
                await context.RespondAsync(res.Exception);
        }
    }
}
