using MassTransit;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Contracts.Classifications.Commands;

namespace Ubik.Accounting.Api.Features.Classifications.Commands
{
    public class AddClassificationConsumer : IConsumer<AddClassificationCommand>
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;

        public AddClassificationConsumer(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
        {
            _serviceManager = serviceManager;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<AddClassificationCommand> context)
        {
            var command = context.Message.ToClassification();

            var result = await _serviceManager.ClassificationService.AddAsync(command);

            await result.Match(
                Right: async r =>
                {
                    //Store and publish AccountGroupAdded event
                    await _publishEndpoint.Publish(r.ToClassificationAdded(), CancellationToken.None);
                    await _serviceManager.SaveAsync();
                    await context.RespondAsync(r.ToAddClassificationResult());
                },
                Left: async err => await context.RespondAsync(err));
        }
    }
}
