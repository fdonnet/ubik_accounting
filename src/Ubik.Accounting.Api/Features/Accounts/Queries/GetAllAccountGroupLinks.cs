using MassTransit;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
using Ubik.Accounting.Contracts.Accounts.Queries;
using Ubik.Accounting.Contracts.Accounts.Results;


namespace Ubik.Accounting.Api.Features.Accounts.Queries
{
    public class GetAllAccountGroupLinksConsumer(IServiceManager serviceManager) : IConsumer<GetAllAccountGroupLinksQuery>
    {
        private readonly IServiceManager _serviceManager = serviceManager;

        public async Task Consume(ConsumeContext<GetAllAccountGroupLinksQuery> context)
        {
            var results = await _serviceManager.AccountService.GetAllAccountGroupLinksAsync();
            await context.RespondAsync<GetAllAccountGroupLinksResults>(new
            {
                AccountGroupLinks = results.ToGetAllAccountGroupLinkResult()
            });
        }
    }
}
