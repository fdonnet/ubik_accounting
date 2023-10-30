using MassTransit;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Contracts.Accounts.Queries;

namespace Ubik.Accounting.Api.Features.Accounts.Queries
{
    /// <summary>
    /// This consumer is only used when called from other microservice
    /// The api client will call service manager directly
    /// </summary>
    public class GetAccountConsumer : IConsumer<GetAccountQuery>
    {
        private readonly IServiceManager _serviceManager;

        public GetAccountConsumer(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task Consume(ConsumeContext<GetAccountQuery> context)
        {
            var result = await _serviceManager.AccountService.GetAsync(context.Message.Id);

            if (result.IsSuccess) 
                await context.RespondAsync(result.Result.ToGetAccountResult());
            else
                await context.RespondAsync(result.Exception);
        }
    }
}
