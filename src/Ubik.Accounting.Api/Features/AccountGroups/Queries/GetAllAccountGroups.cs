using MassTransit;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Contracts.AccountGroups.Queries;
using Ubik.Accounting.Contracts.AccountGroups.Results;

namespace Ubik.Accounting.Api.Features.AccountGroups.Queries
{
    /// <summary>
    /// This consumer is only used when called from other microservice
    /// The api clien will call service manager directly
    /// </summary>
    public class GetAllAccountGroupsConsumer : IConsumer<GetAllAccountGroupsQuery>
    {
        private readonly IServiceManager _serviceManager;

        public GetAllAccountGroupsConsumer(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task Consume(ConsumeContext<GetAllAccountGroupsQuery> context)
        {
            var res = await _serviceManager.AccountGroupService.GetAllAsync();
            await context.RespondAsync<IGetAllAccountGroupsResult>(new
            {
                AccountGroups = res.ToGetAllAccountGroupsResult()
            });
        }
    }
}
