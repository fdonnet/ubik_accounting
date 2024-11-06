using MassTransit;
using Ubik.Accounting.SalesOrVatTax.Contracts.TaxRates.Events;
using Ubik.Accounting.Structure.Contracts.Accounts.Events;
using Ubik.Accounting.Transaction.Api.Features.TaxRates.Services;

namespace Ubik.Accounting.Transaction.Api.Features.TaxRates.Consumers
{
    public class TaxRateDeletedConsumer(ITaxRateCommandService commandService) : IConsumer<TaxRateDeleted>
    {
        public async Task Consume(ConsumeContext<TaxRateDeleted> context)
        {
            await commandService.DeleteAsync(context.Message.Id);
        }
    }
}
