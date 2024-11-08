using MassTransit;
using Ubik.Accounting.SalesOrVatTax.Contracts.TaxRates.Events;
using Ubik.Accounting.Transaction.Api.Features.TaxRates.Services;

namespace Ubik.Accounting.Transaction.Api.Features.TaxRates.Consumers
{
    public class TaxRateUpdatedConsumer(ITaxRateCommandService commandService) : IConsumer<TaxRateUpdated>
    {
        public async Task Consume(ConsumeContext<TaxRateUpdated> context)
        {
            await commandService.UpdateAsync(context.Message);
        }
    }
}

