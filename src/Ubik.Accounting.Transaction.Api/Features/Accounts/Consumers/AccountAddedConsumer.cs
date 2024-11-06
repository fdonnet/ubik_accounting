using MassTransit;
using Ubik.Accounting.Structure.Contracts.Accounts.Events;
using Ubik.Accounting.Transaction.Api.Features.Accounts.Services;

namespace Ubik.Accounting.Transaction.Api.Features.Accounts.Consumers
{
    public class AccountAddedConsumer(IAccountCommandService commandService) : IConsumer<AccountAdded>
    {
        public async Task Consume(ConsumeContext<AccountAdded> context)
        {
            await commandService.AddAsync(context.Message);
        }
    }
}
