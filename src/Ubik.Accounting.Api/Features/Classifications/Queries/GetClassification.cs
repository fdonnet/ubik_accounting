using MassTransit;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Contracts.Classifications.Queries;

namespace Ubik.Accounting.Api.Features.Classifications.Queries
{
    public class GetClassificationConsumer : IConsumer<GetClassificationQuery>
    {
        private readonly IServiceManager _serviceManager;

        public GetClassificationConsumer(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task Consume(ConsumeContext<GetClassificationQuery> context)
        {
            var result = await _serviceManager.ClassificationService.GetAsync(context.Message.Id);

            if (result.IsSuccess)
                await context.RespondAsync(result.Result.ToGetClassificationResult());
            else
                await context.RespondAsync(result.Exception);
        }
    }
}
