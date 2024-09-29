using MassTransit;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Contracts.Classifications.Queries;

namespace Ubik.Accounting.Api.Features.Classifications.Queries
{
    public class GetClassificationConsumer(IServiceManager serviceManager) : IConsumer<GetClassificationQuery>
    {
        public async Task Consume(ConsumeContext<GetClassificationQuery> context)
        {
            var result = await serviceManager.ClassificationService.GetAsync(context.Message.Id);

            await result.Match(
                Right: async ok => await context.RespondAsync(ok.ToGetClassificationResult()),
                Left: async err => await context.RespondAsync(err));
        }
    }
}
