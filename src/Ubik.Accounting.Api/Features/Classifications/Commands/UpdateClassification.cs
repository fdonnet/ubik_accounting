using MassTransit;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Contracts.Classifications.Commands;

namespace Ubik.Accounting.Api.Features.Classifications.Commands
{
    public class UpdateClassificationConsumer : IConsumer<UpdateClassificationCommand>
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;

        public UpdateClassificationConsumer(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
        {
            _serviceManager = serviceManager;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<UpdateClassificationCommand> context)
        {
            var command = context.Message.ToClassification();

            var result = await _serviceManager.ClassificationService.UpdateAsync(command);

            await result.Match(
                Right: async r =>
                {
                    //Store and publish AccountGroupAdded event
                    await _publishEndpoint.Publish(r.ToClassificationUpdated(), CancellationToken.None);
                    await _serviceManager.SaveAsync();
                    await context.RespondAsync(r.ToUpdateClassificationResult());
                },
                Left: async err => await context.RespondAsync(err));
        }
    }
}
