using MassTransit;
using Ubik.Accounting.Structure.Contracts.Accounts.Events;
using Ubik.Accounting.Transaction.Api.Features.Accounts.Services;

namespace Ubik.Accounting.Transaction.Api.Features.Accounts.Consumers
{
    public class AccountUpdatedConsumer(IAccountCommandService commandService) : IConsumer<AccountUpdated>
    {
        public async Task Consume(ConsumeContext<AccountUpdated> context)
        {
            await commandService.UpdateAsync(context.Message);
        }
    }
}
