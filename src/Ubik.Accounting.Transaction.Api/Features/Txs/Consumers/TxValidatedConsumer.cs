using MassTransit;
using Ubik.Accounting.Transaction.Api.Features.Txs.Services;
using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.Accounting.Transaction.Contracts.Txs.Enums;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Consumers
{
    public class TxValidatedConsumer(ITxCommandService commandService) : IConsumer<TxValidated>
    {
        public async Task Consume(ConsumeContext<TxValidated> context)
        {
            var tx = context.Message;
            var result = await commandService.ChangeTxStateAsync(new ChangeTxStateCommand
            {
                TxId = tx.Id,
                State = TxState.Confirmed,
                Version = tx.Version
            });

            if (result.IsLeft)
            {
                throw new Exception("Error while changing tx state");
            }
        }
    }
}
