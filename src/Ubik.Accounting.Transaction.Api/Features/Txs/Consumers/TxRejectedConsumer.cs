using MassTransit;
using Ubik.Accounting.Transaction.Api.Features.Txs.Services;
using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.Accounting.Transaction.Contracts.Txs.Enums;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Consumers
{
    public class TxRejectedConsumer(ITxCommandService commandService) : IConsumer<TxRejected>
    {
        public async Task Consume(ConsumeContext<TxRejected> context)
        {
            var tx = context.Message;
            await commandService.ChangeTxStateAsync(new ChangeTxStateCommand
            {
                TxId = tx.Id,
                State = TxState.Confirmed,
                Version = tx.Version,
                Reason = tx.Reason
            });
        }
    }
}
