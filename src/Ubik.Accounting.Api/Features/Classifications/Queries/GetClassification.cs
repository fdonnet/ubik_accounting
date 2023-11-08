using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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


            await result.Match(
                 Right: async ok => await context.RespondAsync(ok.ToGetClassificationResult()),
                 Left: async err => await context.RespondAsync(err));
        }
    }
}
