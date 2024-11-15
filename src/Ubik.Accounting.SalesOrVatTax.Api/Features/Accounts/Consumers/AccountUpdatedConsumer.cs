using MassTransit;
using Ubik.Accounting.SalesOrVatTax.Api.Features.Accounts.Services;
using Ubik.Accounting.Structure.Contracts.Accounts.Events;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.Accounts.Consumers
{
    public class AccountUpdatedConsumer(IAccountCommandService commandService) : IConsumer<AccountUpdated>
    {
        public async Task Consume(ConsumeContext<AccountUpdated> context)
        {
            await commandService.UpdateAsync(context.Message);
        }
    }
}
