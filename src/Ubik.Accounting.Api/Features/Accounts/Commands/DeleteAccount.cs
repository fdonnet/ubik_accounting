using MassTransit;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Ubik.Accounting.Api.Features.Accounts.Exceptions;
using Ubik.Accounting.Api.Features.Accounts.Mappers;
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
        //Check if the account is found
        var account = await _serviceManager.AccountService.GetAsync(context.Message.Id);

        if (account == null)
        {
            await context.RespondAsync(new AccountNotFoundException(context.Message.Id));
            return;
        }

        await _serviceManager.AccountService.ExecuteDeleteAsync(account.Id);
        await _publishEndpoint.Publish(new AccountDeleted { Id = account.Id }, CancellationToken.None);
        await _serviceManager.SaveAsync();

        await context.RespondAsync <DeleteAccountResult>(new {Deleted = true});
    }
}

