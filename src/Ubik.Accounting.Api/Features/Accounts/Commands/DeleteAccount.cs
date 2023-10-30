using MassTransit;
using Ubik.Accounting.Contracts.Accounts.Commands;
using Ubik.Accounting.Contracts.Accounts.Events;
using Ubik.Accounting.Contracts.Accounts.Results;

namespace Ubik.Accounting.Api.Features.Accounts.Commands;

public class DeleteAccountConsumer : IConsumer<DeleteAccountCommand>
{
    private readonly IServiceManager _serviceManager;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteAccountConsumer(IServiceManager serviceManager, IPublishEndpoint publishEndpoint)
    {
        _serviceManager = serviceManager;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<DeleteAccountCommand> context)
    {
        var res = await _serviceManager.AccountService.ExecuteDeleteAsync(context.Message.Id);

        if(res.IsSuccess)
        {
            await _publishEndpoint.Publish(new AccountDeleted { Id = context.Message.Id }, CancellationToken.None);
            await _serviceManager.SaveAsync();
            await context.RespondAsync<DeleteAccountResult>(new { Deleted = true });
        }
        else
            await context.RespondAsync(res.Exception);
    }
}

