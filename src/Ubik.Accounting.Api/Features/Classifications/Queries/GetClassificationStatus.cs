using MassTransit;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Contracts.Classifications.Queries;

namespace Ubik.Accounting.Api.Features.Classifications.Queries
{
    public class GetClassificationStatusConsumer : IConsumer<GetClassificationStatusQuery>
    {
        private readonly IServiceManager _serviceManager;

        public GetClassificationStatusConsumer(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task Consume(ConsumeContext<GetClassificationStatusQuery> context)
        {
            var result = await _serviceManager.ClassificationService.GetClassificationStatusAsync(context.Message.Id);

            await result.Match(
                Right: async ok => await context.RespondAsync(ok.ToGetClassificationStatusResult()),
                Left: async err => await context.RespondAsync(err));
        }
    }
}
