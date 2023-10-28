﻿using MassTransit;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.Api.Features.Accounts.Queries
{
    public class GetAllAccountsConsumer : IConsumer<GetAllAccountsQuery>
    {
        private readonly IServiceManager _serviceManager;

        public GetAllAccountsConsumer(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task Consume(ConsumeContext<GetAllAccountsQuery> context)
        {
            var accounts = await _serviceManager.AccountService.GetAllAsync();
            await context.RespondAsync<IGetAllAccountsResult>(new
            {
                Accounts = accounts.ToGetAllAccountResult()
            });
        }
    }
}
