using MassTransit;
using Ubik.Accounting.Structure.Contracts.Accounts.Events;
using Ubik.Accounting.Transaction.Api.Features.Accounts.Services;

namespace Ubik.Accounting.Transaction.Api.Features.Accounts.Consumers
{
    public class AccountDeletedConsumer(IAccountCommandService commandService) : IConsumer<AccountDeleted>
    {
        public async Task Consume(ConsumeContext<AccountDeleted> context)
        {
            await commandService.DeleteAsync(context.Message.Id);
        }
    }
}
