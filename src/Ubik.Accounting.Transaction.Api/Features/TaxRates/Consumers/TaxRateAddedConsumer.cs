using MassTransit;
using Ubik.Accounting.SalesOrVatTax.Contracts.TaxRates.Events;
using Ubik.Accounting.Structure.Contracts.Accounts.Events;
using Ubik.Accounting.Transaction.Api.Features.TaxRates.Services;

namespace Ubik.Accounting.Transaction.Api.Features.TaxRates.Consumers
{
    public class TaxRateAddedConsumer(ITaxRateCommandService commandService) : IConsumer<TaxRateAdded>
    {
        public async Task Consume(ConsumeContext<TaxRateAdded> context)
        {
            await commandService.AddAsync(context.Message);
        }
    }
}
