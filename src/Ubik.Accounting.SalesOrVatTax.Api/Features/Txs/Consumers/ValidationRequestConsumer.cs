using MassTransit;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;

namespace Ubik.Accounting.SalesOrVatTax.Api.Features.Txs.Consumers
{
    public class ValidationRequestConsumer(IPublishEndpoint publishEndpoint) : IConsumer<TxTaxValidationRequestSent>
    {
        //TODO: Publish and validation need to be put in a service
        public async Task Consume(ConsumeContext<TxTaxValidationRequestSent> context)
        {
            var tx = context.Message.Tx;
            var random = new Random();
            var i = random.Next(2);

            if(i == 0)
            {
                await publishEndpoint.Publish(new TxValidated
                {
                    Id = tx.Id,
                    Version = tx.Version
                });
            }
            else
            {
                await publishEndpoint.Publish(new TxRejected
                {
                    Id = tx.Id,
                    Version = tx.Version,
                    Reason = "Random tax rejection"
                });
            }
        }
    }
}
