﻿using MassTransit;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.Api.Features.Accounts.Queries
{
    /// <summary>
    /// This consumer is only used when called from other microservice
    /// The api clien will call service manager directly
    /// </summary>
    public class GetAllAccountsConsumer(IServiceManager serviceManager) : IConsumer<GetAllAccountsQuery>
    {
        private readonly IServiceManager _serviceManager = serviceManager;

        public async Task Consume(ConsumeContext<GetAllAccountsQuery> context)
        {
            var accounts = await _serviceManager.AccountService.GetAllAsync();
            await context.RespondAsync<GetAllAccountsResults>(new
            {
                Accounts = accounts.ToGetAllAccountResult()
            });
        }
    }
}
