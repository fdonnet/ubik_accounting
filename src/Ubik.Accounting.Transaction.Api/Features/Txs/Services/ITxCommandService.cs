using LanguageExt;
using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Services
{
    public interface ITxCommandService
    {
        public Task<Either<IFeatureError, TxSubmitted>> SubmitTxAsync(SubmitTxCommand command);
        public bool CheckIfTxNeedTaxValidation(TxSubmitted tx);
        public void PublishValidated(TxValidated tx);
        public Task<TxAdded> AddTxAsync(TxValidated tx);
    }
}
