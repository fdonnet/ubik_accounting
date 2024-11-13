using LanguageExt;
using Ubik.Accounting.Transaction.Api.Models;
using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.Accounting.Transaction.Contracts.Txs.Enums;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Services
{
    public interface ITxCommandService
    {
        public Task<Either<IFeatureError, TxSubmitted>> SubmitTxAsync(SubmitTxCommand command);
        public bool CheckIfTxNeedTaxValidation(TxSubmitted tx);
        public Task<Either<IFeatureError, Tx>> ChangeTxStateAsync(ChangeTxStateCommand command);
        public Task SendTaxValidationRequest(TxSubmitted tx);
    }
}
