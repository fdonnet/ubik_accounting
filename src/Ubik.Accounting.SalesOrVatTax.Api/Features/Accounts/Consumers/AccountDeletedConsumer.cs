using MassTransit;
using Ubik.Accounting.SalesOrVatTax.Api.Features.Accounts.Services;
using Ubik.Accounting.Structure.Contracts.Accounts.Events;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.Accounts.Consumers
{
    public class AccountDeletedConsumer(IAccountCommandService commandService) : IConsumer<AccountDeleted>
    {
        public async Task Consume(ConsumeContext<AccountDeleted> context)
        {
            await commandService.DeleteAsync(context.Message.Id);
        }
    }
}
