using MassTransit;
using Ubik.Accounting.Api.Features.AccountGroups.Mappers;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Contracts.AccountGroups.Queries;
using Ubik.Accounting.Contracts.AccountGroups.Results;
using Ubik.Accounting.Contracts.Classifications.Queries;
using Ubik.Accounting.Contracts.Classifications.Results;

namespace Ubik.Accounting.Api.Features.Classifications.Queries
{
    public class GetAllClassificationsConsumer : IConsumer<GetAllClassificationsQuery>
    {
        private readonly IServiceManager _serviceManager;

        public GetAllClassificationsConsumer(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        public async Task Consume(ConsumeContext<GetAllClassificationsQuery> context)
        {
            var res = await _serviceManager.ClassificationService.GetAllAsync();
            await context.RespondAsync(new GetAllClassificationsResults
            {
                Classifications =res.ToGetAllClassificationsResult()
            });
        }
    }
}
