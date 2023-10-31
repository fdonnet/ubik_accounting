using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using MassTransit;
using Ubik.Accounting.Contracts.AccountGroups.Queries;

namespace Ubik.Accounting.Api.Features.AccountGroups.Queries
{
    /// <summary>
    /// This consumer is only used when called from other microservice
    /// The api client will call service manager directly
    /// </summary>
    public class GetChildAccountsConsumer : IConsumer<GetChildAccountsQuery>
    {
        private readonly IServiceManager _serviceManager;

        public GetChildAccountsConsumer(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task Consume(ConsumeContext<GetChildAccountsQuery> context)
        {
            var result = await _serviceManager.AccountGroupService.GetWithChildAccountsAsync(context.Message.AccountGroupId);

            if (result.IsSuccess)
                await context.RespondAsync(result.Result.Accounts!.ToGetChildAccountsResult());
            else
                await context.RespondAsync(result.Exception);
        }
    }
}
