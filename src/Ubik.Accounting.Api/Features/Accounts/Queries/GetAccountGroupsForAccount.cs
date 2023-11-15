using MassTransit;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Api.Features.Accounts.Queries.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.Api.Features.Accounts.Queries
{
    public class GetAccountGroupsForAccountConsumer : IConsumer<GetAccountGroupsForAccountQuery>
    {
        private readonly IServiceManager _serviceManager;

        public GetAccountGroupsForAccountConsumer(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task Consume(ConsumeContext<GetAccountGroupsForAccountQuery> context)
        {
            var result = await _serviceManager.AccountService.GetAccountGroupsWithClassificationInfoAsync(context.Message.AccountId);

            await result.Match(
                Right: async r => {
                    await context.RespondAsync<GetAccountGroupClassificationResults>(new
                    {
                        AccountGroups = r.ToGetAccountGroupClassificationResult()
                    });
                },
                Left: async err => await context.RespondAsync(err));
        }
    }
}
