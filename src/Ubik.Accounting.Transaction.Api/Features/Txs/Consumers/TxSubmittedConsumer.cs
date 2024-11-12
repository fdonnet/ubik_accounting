using MassTransit;
using Ubik.Accounting.Transaction.Api.Features.Txs.Services;
using Ubik.Accounting.Transaction.Api.Mappers;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Consumers
{
    public class TxSubmittedConsumer(ITxCommandService commandService) : IConsumer<TxSubmitted>
    {
        public async Task Consume(ConsumeContext<TxSubmitted> context)
        {
            var tx = context.Message;

            if(commandService.CheckIfTxNeedTaxValidation(tx))
            {
                //Publish tax validation request

            }
            else
            {
                //Publish validated event (tax validation skipped)
                commandService.PublishValidated(tx.ToTxValidated());
            }
        }
    }
}
