using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Errors
{
    public class ExchangeRateErrors : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public ExchangeRateErrors(IEnumerable<SubmitTxEntry> entriesInError)
        {

            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = new List<CustomError>();

            foreach (var entry in entriesInError)
            {
                CustomErrors.Add(new CustomError()
                {
                    ErrorCode = "BAD_EXCHANGE_RATE_PARAMS_FOR_ENTRY",
                    ErrorFriendlyMessage = "This entry contains invalid exchange rates info.",
                    ErrorValueDetails = $"Field:Amount / Value:{entry.Amount} - Field:OriginalAmount / Value:{entry.AmountAdditionnalInfo?.OriginalAmount} - Field:ExchangeRate / Value:{entry.AmountAdditionnalInfo?.ExchangeRate}"
                });
            }
        }
    }
}
}
