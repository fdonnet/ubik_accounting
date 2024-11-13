using MassTransit;
using Ubik.Accounting.Transaction.Api.Features.Txs.Services;
using Ubik.Accounting.Transaction.Api.Mappers;
using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.Accounting.Transaction.Contracts.Txs.Enums;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Consumers
{
    public class TxSubmittedConsumer(ITxCommandService commandService) : IConsumer<TxSubmitted>
    {
        public async Task Consume(ConsumeContext<TxSubmitted> context)
        {
            var tx = context.Message;
            var needTaxValidation = commandService.CheckIfTxNeedTaxValidation(tx);

            //Need tax validation
            if (needTaxValidation)
                await commandService.SendTaxValidationRequest(tx);

            //Change state
            var result =await commandService.ChangeTxStateAsync(new ChangeTxStateCommand
            {
                TxId = tx.Id,
                State = needTaxValidation
                    ? TxState.WaitingForTaxValidation
                    : TxState.Confirmed,

                Version = tx.Version
            });

            if (result.IsLeft)
            {
                throw new Exception("Error while changing tx state");
            }
        }
    }
}
