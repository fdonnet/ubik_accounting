using LanguageExt;
using Ubik.Accounting.Transaction.Api.Models;
using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Services
{
    public interface ITxCommandService
    {
        public Task<Either<IFeatureError, TxSubmited>> SubmitTx(SubmitTxCommand command);
    }
}
