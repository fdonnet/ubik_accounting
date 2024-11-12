using MassTransit;
using Ubik.Accounting.Transaction.Api.Features.Txs.Services;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Consumers
{
    public class TxValidatedConsumer(ITxCommandService commandService) : IConsumer<TxValidated>
    {
        public async Task Consume(ConsumeContext<TxValidated> context)
        {
            var tx = context.Message;
            await commandService.AddTxAsync(tx);
        }
    }
}
