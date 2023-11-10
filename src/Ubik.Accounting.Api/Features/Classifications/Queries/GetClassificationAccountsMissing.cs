using MassTransit;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Contracts.Classifications.Queries;
using Ubik.Accounting.Contracts.Classifications.Results;

namespace Ubik.Accounting.Api.Features.Classifications.Queries
{
    public class GetClassificationAccountsMissingConsumer : IConsumer<GetClassificationAccountsMissingQuery>
    {
        private readonly IServiceManager _serviceManager;

        public GetClassificationAccountsMissingConsumer(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task Consume(ConsumeContext<GetClassificationAccountsMissingQuery> context)
        {
            var result = await _serviceManager.ClassificationService
                .GetClassificationAccountsMissingAsync(context.Message.ClassificationId);

            await result.Match(
                Right: async ok => await context.RespondAsync(
                    new GetClassificationAccountsMissingResults { Accounts = ok.ToGetClassificationAccountsMissingResult() }),

                Left: async err => await context.RespondAsync(err));
        }
    }
}
