using MassTransit;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Contracts.Classifications.Queries;
using Ubik.Accounting.Contracts.Classifications.Results;

namespace Ubik.Accounting.Api.Features.Classifications.Queries
{
    public class GetClassificationAccountsConsumer : IConsumer<GetClassificationAccountsQuery>
    {
        private readonly IServiceManager _serviceManager;

        public GetClassificationAccountsConsumer(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        public async Task Consume(ConsumeContext<GetClassificationAccountsQuery> context)
        {
            var result = await _serviceManager.ClassificationService
                .GetClassificationAccountsAsync(context.Message.ClassificationId);

            await result.Match(
                Right: async ok => await context.RespondAsync(
                    new GetClassificationAccountsResults { Accounts=ok.ToGetClassificationAccountsResult()}),

                Left: async err => await context.RespondAsync(err));
        }
    }
}
