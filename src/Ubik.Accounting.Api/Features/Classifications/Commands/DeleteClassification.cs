using MassTransit;
using Ubik.Accounting.Api.Features.Classifications.Mappers;
using Ubik.Accounting.Contracts.Classifications.Commands;
using Ubik.Accounting.Contracts.Classifications.Results;

namespace Ubik.Accounting.Api.Features.Classifications.Commands
{
    public class DeleteClassificationConsumer : IConsumer<DeleteClassificationCommand>
    {
        private readonly IServiceManager _serviceManager;
        private readonly IPublishEndpoint _publishEndpoint;

        public DeleteClassificationConsumer(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
        {
            _serviceManager = serviceManager;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<DeleteClassificationCommand> context)
        {
            var res = await _serviceManager.ClassificationService.DeleteAsync(context.Message.Id);

            await res.Match(
                Right: async r =>
                {
                    await _publishEndpoint.Publish(r.ToClassificationDeleted(context.Message.Id), CancellationToken.None);
                    await _serviceManager.SaveAsync();
                    await context.RespondAsync(r.ToDeleteClassificationResult(context.Message.Id));
                },
                Left: async err => await context.RespondAsync(err));
        }
    }
}
