using MassTransit;
using Ubik.Accounting.SalesOrVatTax.Api.Data;
using Ubik.Accounting.SalesOrVatTax.Api.Features.Accounts.Services;
using Ubik.Accounting.Structure.Contracts.Accounts.Events;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.Accounts.Consumers
{
    public class AccountAddedConsumer(IAccountCommandService commandService) : IConsumer<AccountAdded>
    {
        public async Task Consume(ConsumeContext<AccountAdded> context)
        {
            await commandService.AddAsync(context.Message);
        }
    }

    public class AccountAddedFaultConsumer :
        IConsumer<Fault<AccountAdded>>
    {
        public async Task Consume(ConsumeContext<Fault<AccountAdded>> context)
        {
            //TODO: put in place a thing to manage msg faults
        }
    }
}
