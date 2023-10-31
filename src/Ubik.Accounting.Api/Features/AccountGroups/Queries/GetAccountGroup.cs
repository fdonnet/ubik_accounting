using MassTransit;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Contracts.AccountGroups.Queries;

namespace Ubik.Accounting.Api.Features.AccountGroups.Queries
{
    public class GetAccountGroupConsumer : IConsumer<GetAccountGroupQuery>
    {
        private readonly IServiceManager _serviceManager;

        public GetAccountGroupConsumer(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task Consume(ConsumeContext<GetAccountGroupQuery> context)
        {
            var result = await _serviceManager.AccountGroupService.GetAsync(context.Message.Id);

            if (result.IsSuccess)
                await context.RespondAsync(result.Result.ToGetAccountGroupResult());
            else
                await context.RespondAsync(result.Exception);
        }
    }
}
