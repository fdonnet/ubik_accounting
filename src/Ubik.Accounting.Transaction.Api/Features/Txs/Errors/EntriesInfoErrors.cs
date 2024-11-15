using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Errors
{
    public enum EntryErrorType
    {
        ExchangeRate,
        Amount,
        Currency,
    }

    public class EntriesInfoErrors : IFeatureError
    {
        public FeatureErrorType ErrorType { get; init; }
        public List<CustomError> CustomErrors { get; init; }

        public EntriesInfoErrors(Dictionary<EntryErrorType, List<SubmitTxEntry>> entriesInError)
        {
            ErrorType = FeatureErrorType.BadParams;
            CustomErrors = [];

            foreach (var entryGroup in entriesInError)
            {
                foreach (var entry in entryGroup.Value)
                {
                    CustomErrors.Add(new CustomError()
                    {
                        ErrorCode = GetErrorCode(entryGroup.Key),
                        ErrorFriendlyMessage = GetErrorFriendlyMessage(entryGroup.Key),
                        ErrorValueDetails = GetErrorValueDetails(entryGroup.Key, entry)
                    });
                }
            }
        }

        private static string GetErrorCode(EntryErrorType errorType)
        {
            return errorType switch
            {
                EntryErrorType.ExchangeRate => "BAD_EXCHANGE_RATE_PARAMS_FOR_ENTRY",
                EntryErrorType.Amount => "BAD_AMOUNT_PARAMS_FOR_ENTRY",
                EntryErrorType.Currency => "BAD_CURRENCY_PARAMS_FOR_ENTRY",
                _ => "UNKNOWN_ERROR"
            };
        }

        private static string GetErrorFriendlyMessage(EntryErrorType errorType)
        {
            return errorType switch
            {
                EntryErrorType.ExchangeRate => "This entry contains invalid exchange rates info.",
                EntryErrorType.Amount => "This entry contains invalid amount info.",
                EntryErrorType.Currency => "This entry contains invalid currency info.",
                _ => "This entry contains unknown error info."
            };
        }

        private static string GetErrorValueDetails(EntryErrorType errorType, SubmitTxEntry entry)
        {
            return errorType switch
            {
                EntryErrorType.ExchangeRate => $"Field:Amount / Value:{entry.Amount} - Field:OriginalAmount / Value:{entry.AmountAdditionnalInfo?.OriginalAmount} - Field:ExchangeRate / Value:{entry.AmountAdditionnalInfo?.ExchangeRate}",
                EntryErrorType.Amount => $"Field:Amount / Value:{entry.Amount}",
                EntryErrorType.Currency => $"Field:Currency / Value:{entry.AmountAdditionnalInfo?.OriginalCurrencyId}",
                _ => "Unknown error details."
            };
        }
    }
}

